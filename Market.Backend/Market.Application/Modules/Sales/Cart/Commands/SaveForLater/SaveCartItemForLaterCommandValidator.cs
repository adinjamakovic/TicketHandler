namespace Market.Application.Modules.Sales.Cart.Commands.SaveForLater;

public class SaveCartItemForLaterCommandValidator : AbstractValidator<SaveCartItemForLaterCommand>
{
    public SaveCartItemForLaterCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("TicketId must be a positive value.");
    }
}
