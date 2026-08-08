namespace Market.Application.Modules.Sales.Payments.Commands.HandleWebhook
{
    // Raw Stripe webhook delivery. The signature is verified inside the handler, so this
    // is the one command whose input is not trusted at all until then.
    // </summary>
    public class HandlePaymentWebhookCommand : IRequest<Unit>
    {
        public required string Payload { get; set; }
        public string? Signature { get; set; }
    }
}
