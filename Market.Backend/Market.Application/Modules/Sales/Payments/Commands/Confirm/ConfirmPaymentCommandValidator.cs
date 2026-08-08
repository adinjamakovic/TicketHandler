namespace Market.Application.Modules.Sales.Payments.Commands.Confirm;

public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentIntentId)
            .NotEmpty().WithMessage("PaymentIntentId is required.")
            .MaximumLength(255);
    }
}
