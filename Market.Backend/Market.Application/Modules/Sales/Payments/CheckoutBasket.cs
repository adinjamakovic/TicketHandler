using Market.Application.Modules.Sales.Orders;

namespace Market.Application.Modules.Sales.Payments
{
    // Reads the signed-in person's cart and prices it. Both the checkout quote and the
    // payment intent go through here, so the browser can never talk the server into a
    // different amount than the one the tickets actually cost.
    internal static class CheckoutBasket
    {
        public static async Task<CheckoutBasketSummary> BuildAsync(IAppDbContext ctx, int personId, CancellationToken ct)
        {
            var cartItems = await ctx.CartItems
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .OrderBy(x => x.CreatedAtUtc)
                .Select(x => new
                {
                    x.TicketId,
                    x.Quantity,
                    x.Ticket.UnitPrice,
                    x.Ticket.QuantityInStock,
                    EventName = x.Ticket.Event.Name,
                    TicketTypeName = x.Ticket.TicketType.Name
                })
                .ToListAsync(ct);

            if (cartItems.Count == 0)
                throw new MarketBusinessRuleException("CHECKOUT_EMPTY_CART", "Your cart is empty.");

            var lines = new List<CheckoutBasketLine>(cartItems.Count);

            foreach (var cartItem in cartItems)
            {
                if (cartItem.Quantity <= 0)
                    throw new MarketBusinessRuleException(
                        "CHECKOUT_INVALID_QUANTITY",
                        $"Invalid quantity for {cartItem.EventName}.");

                // Stock is re-checked here because it can drop between adding to the cart and paying.
                if (cartItem.Quantity > cartItem.QuantityInStock)
                    throw new MarketBusinessRuleException(
                        "CHECKOUT_OUT_OF_STOCK",
                        $"Only {cartItem.QuantityInStock} ticket(s) left for {cartItem.EventName}.");

                var pricing = OrderPricing.ForLine(cartItem.UnitPrice, cartItem.Quantity);

                lines.Add(new CheckoutBasketLine
                {
                    TicketId = cartItem.TicketId,
                    EventName = cartItem.EventName,
                    TicketTypeName = cartItem.TicketTypeName,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    Subtotal = pricing.Subtotal,
                    DiscountPercent = pricing.DiscountPercent,
                    DiscountAmount = pricing.DiscountAmount,
                    Total = pricing.Total
                });
            }

            return new CheckoutBasketSummary
            {
                Lines = lines,
                Subtotal = OrderPricing.RoundMoney(lines.Sum(x => x.Subtotal)),
                DiscountAmount = OrderPricing.RoundMoney(lines.Sum(x => x.DiscountAmount)),
                Total = OrderPricing.RoundMoney(lines.Sum(x => x.Total)),
                TicketCount = lines.Sum(x => x.Quantity)
            };
        }
    }

    internal sealed class CheckoutBasketSummary
    {
        public required IReadOnlyList<CheckoutBasketLine> Lines { get; init; }
        public required decimal Subtotal { get; init; }
        public required decimal DiscountAmount { get; init; }
        public required decimal Total { get; init; }
        public required decimal TicketCount { get; init; }
    }

    internal sealed class CheckoutBasketLine
    {
        public required int TicketId { get; init; }
        public required string EventName { get; init; }
        public required string TicketTypeName { get; init; }
        public required decimal Quantity { get; init; }
        public required decimal UnitPrice { get; init; }
        public required decimal Subtotal { get; init; }
        public required decimal DiscountPercent { get; init; }
        public required decimal DiscountAmount { get; init; }
        public required decimal Total { get; init; }
    }
}
