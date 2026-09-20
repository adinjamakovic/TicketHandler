using Market.Application.Abstractions.Payments;
using Market.Application.Modules.Sales.Orders;
using Microsoft.Extensions.Logging;

namespace Market.Application.Modules.Sales.Payments
{
    // Applies the outcome of a Stripe payment to our own data.
    // Two independent callers land here — the browser reporting back right after
    // confirming, and the Stripe webhook — so every step has to be safe to run twice.
    // The provider is always asked for the current state of the intent rather than
    // trusting whatever the caller claims happened.
    internal sealed class PaymentSettlementService(
        IAppDbContext ctx,
        IPaymentGateway gateway,
        ILogger<PaymentSettlementService> logger)
    {
        // Room for a couple of short lines about what went wrong; the column matches.
        private const int MaxSettlementIssueLength = 400;

        public async Task<PaymentSettlementResult> SettleAsync(
            string paymentIntentId,
            int? expectedPersonId,
            CancellationToken ct)
        {
            var transaction = await ctx.Transactions
                .FirstOrDefaultAsync(x => x.StripeToken == paymentIntentId, ct);

            if (transaction is null || (expectedPersonId is int personId && transaction.PersonId != personId))
                throw new MarketNotFoundException($"No payment found for intent {paymentIntentId}.");

            var intent = await gateway.GetIntentAsync(paymentIntentId, ct);

            if (OrderSettlement.IsFinal(transaction.Status))
                return Result(transaction, intent);

            switch (intent.Status)
            {
                case PaymentIntentStatus.Succeeded:
                    await FulfillAsync(transaction, intent, ct);
                    break;

                case PaymentIntentStatus.Processing:
                    transaction.Status = OrderStatusType.Confirmed;
                    break;

                case PaymentIntentStatus.Canceled:
                    transaction.Status = OrderStatusType.Cancelled;
                    break;

                default:
                    break;
            }

            try
            {
                await ctx.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                logger.LogInformation(
                    "Payment {PaymentIntentId} was already settled concurrently", paymentIntentId);

                return await ReloadAsync(paymentIntentId, intent, ct);
            }

            return Result(transaction, intent);
        }

        private async Task<PaymentSettlementResult> ReloadAsync(
            string paymentIntentId,
            PaymentIntentDescriptor intent,
            CancellationToken ct)
        {
            var settled = await ctx.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.StripeToken == paymentIntentId, ct)
                ?? throw new MarketNotFoundException($"No payment found for intent {paymentIntentId}.");

            return Result(settled, intent);
        }

        // Turns a succeeded payment into tickets the buyer actually owns. Anything that
        // cannot be completed truthfully parks the transaction in PaymentReview instead of
        // Paid: the money is already ours, so an order must never look fulfilled when the
        // amount is wrong or the stock is not there. Stock only ever moves on the path that
        // ends in Paid, which is the same status the revenue figures count.
        private async Task FulfillAsync(
            TransactionEntity transaction,
            PaymentIntentDescriptor intent,
            CancellationToken ct)
        {
            // The charge happened either way, so record when — even if the rest cannot follow.
            transaction.PaidAt = DateTime.UtcNow;

            if (intent.Amount != transaction.TotalAmount)
            {
                HoldForReview(
                    transaction,
                    intent,
                    $"Provider settled {intent.Amount} {intent.Currency} against an order total of {transaction.TotalAmount}.");

                return;
            }

            var orderItems = await ctx.OrderItems
                .Where(x => x.OrderId == transaction.OrderId)
                .Select(x => new { x.TicketId, x.Quantity })
                .ToListAsync(ct);

            if (orderItems.Count == 0)
            {
                HoldForReview(transaction, intent, "The paid order has no items to fulfil.");

                return;
            }

            // Two lines can point at the same ticket, and it is the total that has to be in
            // stock — checking line by line would let a split order pass and then oversell.
            var demand = orderItems
                .GroupBy(x => x.TicketId)
                .ToDictionary(x => x.Key, x => x.Sum(item => item.Quantity));

            var ticketIds = demand.Keys.ToList();

            var tickets = await ctx.Tickets
                .Where(x => ticketIds.Contains(x.Id))
                .ToListAsync(ct);

            var ticketsById = tickets.ToDictionary(x => x.Id);

            string? shortfall = DescribeShortfall(demand, ticketsById);

            if (shortfall is not null)
            {
                // Nothing is taken out of stock and the cart is left alone: the buyer has not
                // received anything yet, so the lines stay where support can still act on them.
                HoldForReview(transaction, intent, shortfall);

                return;
            }

            foreach (var (ticketId, quantity) in demand)
                ticketsById[ticketId].QuantityInStock -= quantity;

            transaction.Status = OrderSettlement.Settled;

            await ClearPurchasedCartItemsAsync(transaction.PersonId, ticketIds, ct);
        }

        // Names every line we cannot cover, so whoever reviews the order sees the whole
        // problem at once rather than one ticket at a time.
        private static string? DescribeShortfall(
            IReadOnlyDictionary<int, decimal> demand,
            IReadOnlyDictionary<int, TicketsEntity> ticketsById)
        {
            var problems = new List<string>();

            foreach (var (ticketId, quantity) in demand)
            {
                if (!ticketsById.TryGetValue(ticketId, out var ticket))
                {
                    problems.Add($"ticket {ticketId} is no longer on sale");
                    continue;
                }

                if (ticket.QuantityInStock < quantity)
                    problems.Add($"ticket {ticketId} sold {quantity} with {ticket.QuantityInStock} in stock");
            }

            return problems.Count == 0
                ? null
                : $"Cannot fulfil: {string.Join("; ", problems)}.";
        }

        private void HoldForReview(
            TransactionEntity transaction,
            PaymentIntentDescriptor intent,
            string reason)
        {
            transaction.Status = OrderStatusType.PaymentReview;
            transaction.SettlementIssue = reason.Length <= MaxSettlementIssueLength
                ? reason
                : reason[..MaxSettlementIssueLength];

            // Money changed hands and the order cannot complete on its own, so this needs a
            // person — it is logged as an error rather than a warning that scrolls past.
            logger.LogError(
                "Payment {PaymentIntentId} for order {OrderId} held for manual review: {Reason}",
                intent.Id, transaction.OrderId, transaction.SettlementIssue);
        }

        private async Task ClearPurchasedCartItemsAsync(
            int personId,
            IReadOnlyList<int> ticketIds,
            CancellationToken ct)
        {
            var cartItems = await ctx.CartItems
                .Where(x => x.PersonId == personId
                    && ticketIds.Contains(x.TicketId)
                    // Only what was actually bought is cleared; a saved-for-later line was
                    // never part of the order and stays on the shelf.
                    && !x.IsSavedForLater)
                .ToListAsync(ct);

            foreach (var cartItem in cartItems)
            {
                cartItem.IsDeleted = true;
                cartItem.ModifiedAtUtc = DateTime.UtcNow;
            }
        }

        private static PaymentSettlementResult Result(
            TransactionEntity transaction,
            PaymentIntentDescriptor intent) => new()
            {
                OrderId = transaction.OrderId,
                PaymentIntentId = intent.Id,
                Status = intent.Status,
                OrderStatus = transaction.Status,
                Amount = transaction.TotalAmount,
                Currency = intent.Currency,
                PersonId = transaction.PersonId,
                FailureMessage = intent.FailureMessage
            };
    }

    internal sealed class PaymentSettlementResult
    {
        public required int OrderId { get; init; }
        public required string PaymentIntentId { get; init; }
        public required PaymentIntentStatus Status { get; init; }
        public required OrderStatusType OrderStatus { get; init; }
        public required decimal Amount { get; init; }
        public required string Currency { get; init; }
        public required int PersonId { get; init; }
        public string? FailureMessage { get; init; }

        public bool IsPaid => OrderStatus == OrderSettlement.Settled;

        // Charged, but the tickets are not the buyer's yet — not a failure they can retry.
        public bool RequiresReview => OrderStatus == OrderStatusType.PaymentReview;
    }
}
