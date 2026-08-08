using Market.Application.Abstractions.Payments;
using Market.Application.Modules.Sales.Cart;
using Microsoft.Extensions.Logging;

namespace Market.Application.Modules.Sales.Payments.Commands.CreateIntent
{
    public sealed class CreatePaymentIntentCommandHandler(
        IAppDbContext ctx,
        IAppCurrentUser appCurrentUser,
        IPaymentGateway gateway,
        ILogger<CreatePaymentIntentCommandHandler> logger)
        : IRequestHandler<CreatePaymentIntentCommand, CreatePaymentIntentCommandDto>
    {
        public async Task<CreatePaymentIntentCommandDto> Handle(
            CreatePaymentIntentCommand req,
            CancellationToken ct)
        {
            int personId = CartPerson.RequireId(appCurrentUser);

            if (!gateway.IsConfigured)
                throw new MarketBusinessRuleException(
                    "PAYMENTS_NOT_CONFIGURED",
                    "Card payments are unavailable at the moment.");

            var basket = await CheckoutBasket.BuildAsync(ctx, personId, ct);

            // A declined card sends the buyer straight back to the same form. Picking the
            // unpaid attempt back up keeps that from piling up abandoned orders.
            var reusable = await FindReusableAttemptAsync(personId, basket, ct);
            if (reusable is not null)
                return reusable;

            var order = CreateOrder(personId, basket);

            await ctx.SaveChangesAsync(ct);

            PaymentIntentDescriptor intent;

            try
            {
                intent = await gateway.CreateIntentAsync(
                    new CreatePaymentIntentRequest
                    {
                        Amount = basket.Total,
                        Currency = gateway.Currency,
                        OrderId = order.Id,
                        PersonId = personId,
                        Description = $"Order #{order.Id} — {basket.Lines.Count} ticket type(s)",
                        ReceiptEmail = req.BillingDetails.Email,
                        Billing = await BuildBillingAsync(req.BillingDetails, ct),
                        Note = req.Note
                    },
                    ct);
            }
            catch
            {
                // Stripe never accepted the order, so it must not linger as an unpayable draft.
                await DiscardOrderAsync(order, ct);
                throw;
            }

            ctx.Transactions.Add(new TransactionEntity
            {
                OrderId = order.Id,
                PersonId = personId,
                Status = OrderStatusType.Draft,
                TotalAmount = basket.Total,
                StripeToken = intent.Id
            });

            await ctx.SaveChangesAsync(ct);

            return ToDto(order.Id, intent, basket.Total);
        }

        private OrderEntity CreateOrder(int personId, CheckoutBasketSummary basket)
        {
            var order = new OrderEntity { PersonId = personId };

            ctx.Orders.Add(order);

            foreach (var line in basket.Lines)
            {
                ctx.OrderItems.Add(new OrderItemEntity
                {
                    Order = order,
                    TicketId = line.TicketId,
                    Quantity = line.Quantity,
                    Subtotal = line.Subtotal,
                    DiscountPercent = line.DiscountPercent,
                    DiscountAmount = line.DiscountAmount,
                    Total = line.Total
                });
            }

            return order;
        }

        private async Task<CreatePaymentIntentCommandDto?> FindReusableAttemptAsync(
            int personId,
            CheckoutBasketSummary basket,
            CancellationToken ct)
        {
            var candidate = await ctx.Transactions
                .AsNoTracking()
                .Where(x => x.PersonId == personId
                            && x.Status == OrderStatusType.Draft
                            && x.TotalAmount == basket.Total)
                .OrderByDescending(x => x.CreatedAtUtc)
                .FirstOrDefaultAsync(ct);

            if (candidate is null)
                return null;

            var orderItems = await ctx.OrderItems
                .AsNoTracking()
                .Where(x => x.OrderId == candidate.OrderId)
                .Select(x => new { x.TicketId, x.Quantity })
                .ToListAsync(ct);

            bool sameBasket = orderItems.Count == basket.Lines.Count
                && basket.Lines.All(line => orderItems.Any(
                    item => item.TicketId == line.TicketId && item.Quantity == line.Quantity));

            if (!sameBasket)
                return null;

            PaymentIntentDescriptor intent;

            try
            {
                intent = await gateway.GetIntentAsync(candidate.StripeToken, ct);
            }
            catch (MarketBusinessRuleException ex)
            {
                // The intent is gone or unreadable — fall through and open a fresh one.
                logger.LogWarning(ex,
                    "Could not reuse payment {PaymentIntentId} for order {OrderId}",
                    candidate.StripeToken, candidate.OrderId);

                return null;
            }

            bool isStillPayable = intent.Status is PaymentIntentStatus.RequiresPaymentMethod
                or PaymentIntentStatus.RequiresConfirmation
                or PaymentIntentStatus.RequiresAction;

            if (!isStillPayable || string.IsNullOrWhiteSpace(intent.ClientSecret))
                return null;

            return ToDto(candidate.OrderId, intent, candidate.TotalAmount);
        }

        private async Task<PaymentBillingDetails> BuildBillingAsync(
            CreatePaymentIntentCommandBillingDetails billing,
            CancellationToken ct)
        {
            var isoCode = await ctx.Countries
                .AsNoTracking()
                .Where(x => x.Id == billing.CountryId)
                .Select(x => x.IsoCode)
                .FirstOrDefaultAsync(ct);

            if (isoCode is null)
                throw new MarketNotFoundException($"Country with Id {billing.CountryId} does not exist.");

            return new PaymentBillingDetails
            {
                Name = $"{billing.FirstName} {billing.LastName}".Trim(),
                Email = billing.Email,
                Phone = billing.Phone,
                Line1 = billing.AddressLine1,
                Line2 = billing.AddressLine2,
                City = billing.City,
                State = billing.State,
                PostalCode = billing.PostalCode,
                CountryIsoCode = isoCode
            };
        }

        private async Task DiscardOrderAsync(OrderEntity order, CancellationToken ct)
        {
            var orderItems = await ctx.OrderItems
                .Where(x => x.OrderId == order.Id)
                .ToListAsync(ct);

            // Remove() is turned into a soft delete by the context's audit hook.
            ctx.OrderItems.RemoveRange(orderItems);
            ctx.Orders.Remove(order);

            await ctx.SaveChangesAsync(ct);
        }

        private static CreatePaymentIntentCommandDto ToDto(
            int orderId,
            PaymentIntentDescriptor intent,
            decimal amount) => new()
            {
                OrderId = orderId,
                PaymentIntentId = intent.Id,
                ClientSecret = intent.ClientSecret!,
                Amount = amount,
                Currency = intent.Currency
            };
    }
}
