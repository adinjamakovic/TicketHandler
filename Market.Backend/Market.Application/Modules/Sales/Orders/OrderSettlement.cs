namespace Market.Application.Modules.Sales.Orders
{
    // The one definition of "this order is really paid for". Settlement is the only thing
    // that writes it; fulfilment, stock and the revenue figures all read it, so nothing can
    // count an order the payment never actually settled.
    internal static class OrderSettlement
    {
        public const OrderStatusType Settled = OrderStatusType.Paid;

        // Once a payment reaches one of these, only a person moves it on — a retrying webhook
        // must not undo a refund or re-run a fulfilment support has already sorted out.
        public static bool IsFinal(OrderStatusType status) => status is Settled
            or OrderStatusType.Completed
            or OrderStatusType.Cancelled
            or OrderStatusType.PaymentReview;

        // Orders backed by a settled payment — the only ones that are real money.
        public static IQueryable<int> SettledOrderIds(IAppDbContext ctx) =>
            ctx.Transactions
                .Where(x => x.Status == Settled)
                .Select(x => x.OrderId);
    }
}
