using Market.Application.Abstractions.Payments;
using Microsoft.Extensions.Logging;

namespace Market.Application.Modules.Sales.Payments.Commands.HandleWebhook
{
    internal sealed class HandlePaymentWebhookCommandHandler(
        IPaymentGateway gateway,
        PaymentSettlementService settlement,
        ILogger<HandlePaymentWebhookCommandHandler> logger)
        : IRequestHandler<HandlePaymentWebhookCommand, Unit>
    { 
        private static readonly HashSet<string> HandledEventTypes =
        [
            "payment_intent.succeeded",
            "payment_intent.payment_failed",
            "payment_intent.canceled",
            "payment_intent.processing"
        ];

        public async Task<Unit> Handle(HandlePaymentWebhookCommand req, CancellationToken ct)
        {
            PaymentWebhookEvent webhookEvent = gateway.ParseWebhookEvent(req.Payload, req.Signature);

            if (!HandledEventTypes.Contains(webhookEvent.Type) || webhookEvent.PaymentIntentId is null)
            {
                logger.LogDebug("Ignoring Stripe event {EventType}", webhookEvent.Type);

                return Unit.Value;
            }

            try
            {
                var result = await settlement.SettleAsync(webhookEvent.PaymentIntentId, null, ct);

                logger.LogInformation(
                    "Stripe event {EventType} left order {OrderId} as {OrderStatus}",
                    webhookEvent.Type, result.OrderId, result.OrderStatus);
            }
            catch (MarketNotFoundException)
            {
                logger.LogWarning(
                    "Stripe event {EventType} referenced unknown payment {PaymentIntentId}",
                    webhookEvent.Type, webhookEvent.PaymentIntentId);
            }

            return Unit.Value;
        }
    }
}
