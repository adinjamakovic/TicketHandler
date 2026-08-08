namespace Market.Application.Modules.Sales.Payments.Commands.CreateIntent
{
    // Turns the signed-in person's cart into an order and opens a Stripe payment for it.
    // Deliberately carries no prices or line items: the amount is derived from the cart
    // on the server, so the browser cannot influence what gets charged.
    public class CreatePaymentIntentCommand : IRequest<CreatePaymentIntentCommandDto>
    {
        public required CreatePaymentIntentCommandBillingDetails BillingDetails { get; set; }
        public string? Note { get; set; }
    }

    // Everything Stripe is given as the payment method's billing details. The checkout form
    // collects all of it, so only the second address line may be left out.
    public class CreatePaymentIntentCommandBillingDetails
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required int CountryId { get; set; }
    }
}
