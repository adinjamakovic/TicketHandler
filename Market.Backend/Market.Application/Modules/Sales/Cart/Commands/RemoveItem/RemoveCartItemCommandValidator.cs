namespace Market.Application.Modules.Sales.Cart.Commands.RemoveItem;

public class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("TicketId must be a positive value.");
    }
}
