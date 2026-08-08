using Market.Application.Abstractions.Payments;
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

            if (transaction.Status == OrderStatusType.Paid)
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

        private async Task FulfillAsync(
            TransactionEntity transaction,
            PaymentIntentDescriptor intent,
            CancellationToken ct)
        {
            if (intent.Amount != transaction.TotalAmount)
                logger.LogWarning(
                    "Payment {PaymentIntentId} settled {PaidAmount} but order {OrderId} totals {OrderAmount}",
                    intent.Id, intent.Amount, transaction.OrderId, transaction.TotalAmount);

            transaction.Status = OrderStatusType.Paid;
            transaction.PaidAt = DateTime.UtcNow;

            var orderItems = await ctx.OrderItems
                .Where(x => x.OrderId == transaction.OrderId)
                .Select(x => new { x.TicketId, x.Quantity })
                .ToListAsync(ct);

            if (orderItems.Count == 0)
            {
                logger.LogWarning(
                    "Order {OrderId} was paid but has no items to fulfil", transaction.OrderId);

                return;
            }

            await ReleaseStockAsync(orderItems.Select(x => (x.TicketId, x.Quantity)).ToList(), ct);
            await ClearPurchasedCartItemsAsync(
                transaction.PersonId,
                orderItems.Select(x => x.TicketId).ToList(),
                ct);
        }

        private async Task ReleaseStockAsync(
            IReadOnlyList<(int TicketId, decimal Quantity)> soldItems,
            CancellationToken ct)
        {
            var ticketIds = soldItems.Select(x => x.TicketId).ToList();

            var tickets = await ctx.Tickets
                .Where(x => ticketIds.Contains(x.Id))
                .ToListAsync(ct);

            var ticketsById = tickets.ToDictionary(x => x.Id);

            foreach (var (ticketId, quantity) in soldItems)
            {
                if (!ticketsById.TryGetValue(ticketId, out var ticket))
                    continue;

                if (ticket.QuantityInStock < quantity)
                    // Oversold: the payment already succeeded, so this is a support problem,
                    // not something to fail the request over.
                    logger.LogWarning(
                        "Ticket {TicketId} oversold — {Sold} sold with {InStock} in stock",
                        ticketId, quantity, ticket.QuantityInStock);

                ticket.QuantityInStock = Math.Max(0, ticket.QuantityInStock - quantity);
            }
        }
        private async Task ClearPurchasedCartItemsAsync(
            int personId,
            IReadOnlyList<int> ticketIds,
            CancellationToken ct)
        {
            var cartItems = await ctx.CartItems
                .Where(x => x.PersonId == personId && ticketIds.Contains(x.TicketId))
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

        public bool IsPaid => OrderStatus == OrderStatusType.Paid;
    }
}
