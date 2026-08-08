namespace Market.Application.Modules.Sales.Payments.Commands.Confirm
{
    // Called by the browser once Stripe.js reports back, so the buyer sees a settled
    // order without waiting for the webhook. The outcome is still read from Stripe —
    // the browser only says which payment to look at.
    public class ConfirmPaymentCommand : IRequest<ConfirmPaymentCommandDto>
    {
        public required string PaymentIntentId { get; set; }
    }
}
