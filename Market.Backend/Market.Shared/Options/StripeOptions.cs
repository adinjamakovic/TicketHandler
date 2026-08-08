namespace Market.Shared.Options;

// Stripe configuration, bound from the "Stripe" section.
public sealed class StripeOptions
{
    public const string SectionName = "Stripe";
    public string PublishableKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;

    // Signing secret (whsec_...) for POST /Payments/webhook. Obtained from the Stripe
    // dashboard, or from `stripe listen --forward-to ...` when developing locally.
    public string WebhookSecret { get; init; } = string.Empty;
    // ISO 4217 currency charges are created in. Must match the currency the prices
    // in the catalogue are expressed in, because ticket prices are sent to Stripe as-is.
    public string Currency { get; init; } = "BAM";
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(SecretKey) && !string.IsNullOrWhiteSpace(PublishableKey);
}
