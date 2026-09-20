using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Commands.MoveToCart
{
    // The counterpart of saving for later. A parked line can sit there for weeks, so the
    // stock it was parked with is re-checked before it is allowed back into the cart.
    public class MoveCartItemToCartCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<MoveCartItemToCartCommand, Unit>
    {
        public async Task<Unit> Handle(MoveCartItemToCartCommand req, CancellationToken ct)
        {
            int personId = CartPerson.RequireId(appCurrentUser);

            var cartItem = await ctx.CartItems
                .FirstOrDefaultAsync(x => x.PersonId == personId && x.TicketId == req.TicketId, ct);

            if (cartItem is null)
                throw new MarketNotFoundException($"Ticket with Id {req.TicketId} is not saved for later");

            if (!cartItem.IsSavedForLater)
                return Unit.Value;

            var ticket = await ctx.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == req.TicketId, ct);

            if (ticket is null)
                throw new MarketNotFoundException($"Ticket with Id {req.TicketId} does not exist");

            if (cartItem.Quantity > ticket.QuantityInStock)
                throw new MarketBusinessRuleException(
                    "CART_OUT_OF_STOCK",
                    $"Only {ticket.QuantityInStock} ticket(s) left, but you saved {cartItem.Quantity}.");

            cartItem.IsSavedForLater = false;
            cartItem.ModifiedAtUtc = DateTime.UtcNow;

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
