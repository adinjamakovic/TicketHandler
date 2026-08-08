namespace Market.Application.Abstractions.Payments;

public sealed record PaymentBillingDetails
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Line1 { get; init; }
    public string? Line2 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? PostalCode { get; init; }
    // ISO 3166-1 alpha-2, e.g. "BA".
    public string? CountryIsoCode { get; init; }
}
public sealed record CreatePaymentIntentRequest
{
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required int OrderId { get; init; }
    public required int PersonId { get; init; }
    public string? Description { get; init; }
    public string? ReceiptEmail { get; init; }
    public PaymentBillingDetails? Billing { get; init; }
    public string? Note { get; init; }
}

public sealed record PaymentIntentDescriptor
{
    public required string Id { get; init; }
    public string? ClientSecret { get; init; }
    public required PaymentIntentStatus Status { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public int? OrderId { get; init; }
    public string? FailureMessage { get; init; }
}
public sealed record PaymentWebhookEvent
{
    public required string Type { get; init; }
    public string? PaymentIntentId { get; init; }
}
