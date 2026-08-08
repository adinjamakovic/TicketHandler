using FluentValidation;
using Market.Application.Abstractions.Payments;
using Market.Application.Common.Exceptions;
using Market.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace Market.Infrastructure.Common;

public sealed class StripePaymentGateway : IPaymentGateway
{
    // Currencies Stripe expects in whole units instead of cents.
    // https://docs.stripe.com/currencies#zero-decimal
    private static readonly HashSet<string> ZeroDecimalCurrencies = new(StringComparer.OrdinalIgnoreCase)
    {
        "bif", "clp", "djf", "gnf", "jpy", "kmf", "krw", "mga",
        "pyg", "rwf", "ugx", "vnd", "vuv", "xaf", "xof", "xpf"
    };
    private readonly StripeOptions _options;
    private readonly ILogger<StripePaymentGateway> _logger;
    private readonly PaymentIntentService _intents;
    public StripePaymentGateway(IOptions<StripeOptions> options, ILogger<StripePaymentGateway> logger)
    {
        _options = options.Value;
        _logger = logger;
        _intents = new PaymentIntentService(new StripeClient(_options.SecretKey));
    }
    public bool IsConfigured => _options.IsConfigured;
    public string PublishableKey => _options.PublishableKey;
    public string Currency => _options.Currency;

    public async Task<PaymentIntentDescriptor> CreateIntentAsync(
        CreatePaymentIntentRequest request,
        CancellationToken ct)
    {
        RequireConfigured();

        var options = new PaymentIntentCreateOptions
        {
            Amount = ToMinorUnits(request.Amount, request.Currency),
            Currency = request.Currency.ToLowerInvariant(),
            Description = request.Description,
            ReceiptEmail = NullIfBlank(request.ReceiptEmail),
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
            Metadata = BuildMetadata(request)
        };

        try
        {
            var intent = await _intents.CreateAsync(
                options,
                new RequestOptions { IdempotencyKey = $"order-{request.OrderId}" },
                ct);

            return ToDescriptor(intent);
        }
        catch (StripeException ex)
        {
            throw Translate(ex, $"creating a payment for order {request.OrderId}");
        }
    }

    public async Task<PaymentIntentDescriptor> GetIntentAsync(string paymentIntentId, CancellationToken ct)
    {
        RequireConfigured();

        try
        {
            var intent = await _intents.GetAsync(paymentIntentId, cancellationToken: ct);

            return ToDescriptor(intent);
        }
        catch (StripeException ex)
        {
            throw Translate(ex, $"reading payment {paymentIntentId}");
        }
    }

    public async Task CancelIntentAsync(string paymentIntentId, CancellationToken ct)
    {
        RequireConfigured();

        try
        {
            await _intents.CancelAsync(paymentIntentId, cancellationToken: ct);
        }
        catch (StripeException ex)
        {
            // Cancelling is always best-effort cleanup, never the reason a request fails.
            _logger.LogWarning(ex, "Could not cancel Stripe payment intent {PaymentIntentId}", paymentIntentId);
        }
    }

    public PaymentWebhookEvent ParseWebhookEvent(string payload, string? signatureHeader)
    {
        if (string.IsNullOrWhiteSpace(_options.WebhookSecret))
            throw new MarketBusinessRuleException(
                "PAYMENTS_WEBHOOK_NOT_CONFIGURED",
                "Stripe:WebhookSecret is not configured, webhook events cannot be verified.");

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                payload,
                signatureHeader,
                _options.WebhookSecret,
                throwOnApiVersionMismatch: false);
        }
        catch (Exception ex) when (ex is not MarketBusinessRuleException)
        {
            // This endpoint is anonymous, so anything can be posted at it. Bad signatures
            // and unparseable bodies both answer 400 — what Stripe reads as "do not retry" —
            // rather than surfacing as a server error.
            _logger.LogWarning(ex, "Rejected a Stripe webhook delivery");

            throw new ValidationException($"Invalid Stripe webhook payload: {ex.Message}");
        }

        return new PaymentWebhookEvent
        {
            Type = stripeEvent.Type,
            PaymentIntentId = (stripeEvent.Data.Object as PaymentIntent)?.Id
        };
    }

    private void RequireConfigured()
    {
        if (!IsConfigured)
            throw new MarketBusinessRuleException(
                "PAYMENTS_NOT_CONFIGURED",
                "Card payments are unavailable because the Stripe keys are not configured.");
    }

    private static Dictionary<string, string> BuildMetadata(CreatePaymentIntentRequest request)
    {
        var metadata = new Dictionary<string, string>
        {
            ["orderId"] = request.OrderId.ToString(),
            ["personId"] = request.PersonId.ToString()
        };

        var billing = request.Billing;
        if (billing is not null)
        {
            AddIfPresent(metadata, "billingName", billing.Name);
            AddIfPresent(metadata, "billingPhone", billing.Phone);
            AddIfPresent(metadata, "billingAddress", FormatAddress(billing));
        }

        AddIfPresent(metadata, "note", request.Note);

        return metadata;
    }

    private static string FormatAddress(PaymentBillingDetails billing)
    {
        var parts = new[]
        {
            billing.Line1,
            billing.Line2,
            billing.PostalCode,
            billing.City,
            billing.State,
            billing.CountryIsoCode
        };

        return string.Join(", ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private static void AddIfPresent(IDictionary<string, string> metadata, string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        // Stripe rejects metadata values longer than 500 characters.
        metadata[key] = value.Length > 500 ? value[..500] : value;
    }

    private static PaymentIntentDescriptor ToDescriptor(PaymentIntent intent) => new()
    {
        Id = intent.Id,
        ClientSecret = intent.ClientSecret,
        Status = ToStatus(intent.Status),
        Amount = FromMinorUnits(intent.Amount, intent.Currency),
        Currency = intent.Currency?.ToUpperInvariant() ?? string.Empty,
        OrderId = intent.Metadata is not null
                  && intent.Metadata.TryGetValue("orderId", out var orderId)
                  && int.TryParse(orderId, out var parsedOrderId)
            ? parsedOrderId
            : null,
        FailureMessage = intent.LastPaymentError?.Message
    };

    private static PaymentIntentStatus ToStatus(string? status) => status switch
    {
        "requires_payment_method" => PaymentIntentStatus.RequiresPaymentMethod,
        "requires_confirmation" => PaymentIntentStatus.RequiresConfirmation,
        "requires_action" => PaymentIntentStatus.RequiresAction,
        "processing" => PaymentIntentStatus.Processing,
        "requires_capture" => PaymentIntentStatus.RequiresCapture,
        "succeeded" => PaymentIntentStatus.Succeeded,
        "canceled" => PaymentIntentStatus.Canceled,
        _ => PaymentIntentStatus.RequiresPaymentMethod
    };

    private static long ToMinorUnits(decimal amount, string currency) =>
        ZeroDecimalCurrencies.Contains(currency)
            ? (long)Math.Round(amount, 0, MidpointRounding.AwayFromZero)
            : (long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero);

    private static decimal FromMinorUnits(long amount, string? currency) =>
        currency is not null && ZeroDecimalCurrencies.Contains(currency)
            ? amount
            : amount / 100m;

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    private MarketBusinessRuleException Translate(StripeException ex, string whileDoing)
    {
        _logger.LogError(ex, "Stripe rejected the request while {WhileDoing}", whileDoing);

        return new MarketBusinessRuleException(
            "PAYMENT_PROVIDER_ERROR",
            ex.StripeError?.Message ?? $"The payment provider rejected the request while {whileDoing}.",
            ex);
    }
}
