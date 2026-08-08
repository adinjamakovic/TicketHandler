namespace Market.Application.Modules.Sales.Orders
{
    // The one place order money is calculated, so the amount shown at checkout, the amount
    // charged by Stripe and the amount stored on the order can never drift apart.
    internal static class OrderPricing
    {
        public const decimal DiscountPercent = 0.05m;
        public static decimal RoundMoney(decimal value) =>
            Math.Round(value, 2, MidpointRounding.AwayFromZero);

        public static OrderLinePricing ForLine(decimal unitPrice, decimal quantity)
        {
            decimal subtotal = RoundMoney(unitPrice * quantity);
            decimal discountAmount = RoundMoney(subtotal * DiscountPercent);

            return new OrderLinePricing(subtotal, DiscountPercent, discountAmount, RoundMoney(subtotal - discountAmount));
        }
    }

    internal sealed record OrderLinePricing(
        decimal Subtotal,
        decimal DiscountPercent,
        decimal DiscountAmount,
        decimal Total);
}
