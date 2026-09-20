namespace Market.Application.Modules.Sales.Cart.Commands.MoveToCart;

public class MoveCartItemToCartCommandValidator : AbstractValidator<MoveCartItemToCartCommand>
{
    public MoveCartItemToCartCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("TicketId must be a positive value.");
    }
}
