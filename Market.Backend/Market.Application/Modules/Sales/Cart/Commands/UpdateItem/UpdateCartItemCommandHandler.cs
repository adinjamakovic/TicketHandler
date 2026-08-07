using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Commands.UpdateItem
{
    public class UpdateCartItemCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<UpdateCartItemCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateCartItemCommand req, CancellationToken ct)
        {
            int personId = CartPerson.RequireId(appCurrentUser);

            var cartItem = await ctx.CartItems
                .FirstOrDefaultAsync(x => x.PersonId == personId && x.TicketId == req.TicketId, ct);

            if (cartItem is null)
                throw new MarketNotFoundException($"Ticket with Id {req.TicketId} is not in your cart");

            var ticket = await ctx.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == req.TicketId, ct);

            if (ticket is null)
                throw new MarketNotFoundException($"Ticket with Id {req.TicketId} does not exist");

            if (req.Quantity > ticket.QuantityInStock)
                throw new MarketBusinessRuleException(
                    "CART_OUT_OF_STOCK",
                    $"Only {ticket.QuantityInStock} ticket(s) left for this event.");

            cartItem.Quantity = req.Quantity;
            cartItem.ModifiedAtUtc = DateTime.UtcNow;

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
