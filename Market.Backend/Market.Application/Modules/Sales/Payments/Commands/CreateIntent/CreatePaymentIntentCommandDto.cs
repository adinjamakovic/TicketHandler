namespace Market.Application.Modules.Sales.Payments.Commands.CreateIntent
{
    public sealed class CreatePaymentIntentCommandDto
    {
        public required int OrderId { get; init; }
        public required string PaymentIntentId { get; init; }
        public required string ClientSecret { get; init; }
        public required decimal Amount { get; init; }
        public required string Currency { get; init; }
    }
}
