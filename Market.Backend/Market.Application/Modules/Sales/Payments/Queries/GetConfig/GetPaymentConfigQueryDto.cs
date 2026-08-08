namespace Market.Application.Modules.Sales.Payments.Queries.GetConfig
{
    public sealed class GetPaymentConfigQueryDto
    {
        public required string PublishableKey { get; init; }
        // ISO 4217 currency every charge is created in.
        public required string Currency { get; init; }
        // False when the server has no Stripe keys — the UI hides the card form.
        public required bool IsConfigured { get; init; }
    }
}
