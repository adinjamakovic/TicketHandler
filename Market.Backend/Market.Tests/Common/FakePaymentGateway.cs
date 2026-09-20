using Market.Application.Abstractions.Payments;

namespace Market.Tests.Common;

// Stand-in for Stripe. Settlement never trusts its caller — it asks the gateway what
// really happened — so tests set that answer here.
public sealed class FakePaymentGateway : IPaymentGateway
{
    public bool IsConfigured => true;
    public string PublishableKey => "pk_test_fake";
    public string Currency => "BAM";

    public required PaymentIntentDescriptor Intent { get; set; }

    public static FakePaymentGateway Reporting(
        string paymentIntentId,
        PaymentIntentStatus status,
        decimal amount,
        string? failureMessage = null) => new()
        {
            Intent = new PaymentIntentDescriptor
            {
                Id = paymentIntentId,
                Status = status,
                Amount = amount,
                Currency = "BAM",
                FailureMessage = failureMessage
            }
        };

    public static FakePaymentGateway Succeeded(string paymentIntentId, decimal amount) =>
        Reporting(paymentIntentId, PaymentIntentStatus.Succeeded, amount);

    public Task<PaymentIntentDescriptor> GetIntentAsync(string paymentIntentId, CancellationToken ct) =>
        Task.FromResult(Intent);

    public Task<PaymentIntentDescriptor> CreateIntentAsync(CreatePaymentIntentRequest request, CancellationToken ct) =>
        Task.FromResult(Intent);

    public Task CancelIntentAsync(string paymentIntentId, CancellationToken ct) => Task.CompletedTask;

    public PaymentWebhookEvent ParseWebhookEvent(string payload, string? signatureHeader) =>
        new() { Type = "payment_intent.succeeded", PaymentIntentId = Intent.Id };
}
