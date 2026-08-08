namespace Market.Application.Modules.Sales.Payments.Commands.CreateIntent;

public class CreatePaymentIntentCommandValidator : AbstractValidator<CreatePaymentIntentCommand>
{
    public CreatePaymentIntentCommandValidator()
    {
        RuleFor(x => x.BillingDetails)
            .NotNull().WithMessage("Billing details are required.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Note is too long (max 500 characters).");

        When(x => x.BillingDetails is not null, () =>
        {
            RuleFor(x => x.BillingDetails.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100);

            RuleFor(x => x.BillingDetails.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100);

            RuleFor(x => x.BillingDetails.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Enter a valid email address.")
                .MaximumLength(200);

            RuleFor(x => x.BillingDetails.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(30);

            RuleFor(x => x.BillingDetails.AddressLine1)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200);

            RuleFor(x => x.BillingDetails.AddressLine2)
                .MaximumLength(200);

            RuleFor(x => x.BillingDetails.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100);

            RuleFor(x => x.BillingDetails.State)
                .NotEmpty().WithMessage("State / region is required.")
                .MaximumLength(100);

            RuleFor(x => x.BillingDetails.PostalCode)
                .NotEmpty().WithMessage("Postal code is required.")
                .MaximumLength(20);

            RuleFor(x => x.BillingDetails.CountryId)
                .GreaterThan(0).WithMessage("Country is required.");
        });
    }
}
