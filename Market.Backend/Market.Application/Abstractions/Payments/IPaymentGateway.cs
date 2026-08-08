namespace Market.Application.Abstractions.Payments;

public interface IPaymentGateway
{
    bool IsConfigured { get; }
    string PublishableKey { get; }
    // ISO 4217 currency every charge is created in
    string Currency { get; }
    Task<PaymentIntentDescriptor> CreateIntentAsync(CreatePaymentIntentRequest request, CancellationToken ct);
    Task<PaymentIntentDescriptor> GetIntentAsync(string paymentIntentId, CancellationToken ct);
    Task CancelIntentAsync(string paymentIntentId, CancellationToken ct);
    PaymentWebhookEvent ParseWebhookEvent(string payload, string? signatureHeader);
}
