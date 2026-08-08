namespace Market.Application.Modules.Sales.Payments.Queries.GetQuote
{
    public sealed class GetPaymentQuoteQueryDto
    {
        public required string Currency { get; init; }
        public required int LineCount { get; init; }
        public required decimal TicketCount { get; init; }
        public required decimal Subtotal { get; init; }
        public required decimal DiscountAmount { get; init; }
        public required decimal Total { get; init; }
        public required List<GetPaymentQuoteQueryDtoLine> Lines { get; init; }
    }

    public sealed class GetPaymentQuoteQueryDtoLine
    {
        public required int TicketId { get; init; }
        public required string EventName { get; init; }
        public required string TicketTypeName { get; init; }
        public required decimal Quantity { get; init; }
        public required decimal UnitPrice { get; init; }
        public required decimal Subtotal { get; init; }
        public required decimal DiscountAmount { get; init; }
        public required decimal Total { get; init; }
    }
}
