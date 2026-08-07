using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Commands.RemoveItem
{
    public class RemoveCartItemCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<RemoveCartItemCommand, Unit>
    {
        public async Task<Unit> Handle(RemoveCartItemCommand req, CancellationToken ct)
        {
            int personId = CartPerson.RequireId(appCurrentUser);

            var cartItem = await ctx.CartItems
                .FirstOrDefaultAsync(x => x.PersonId == personId && x.TicketId == req.TicketId, ct);

            if (cartItem is null)
                throw new MarketNotFoundException($"Ticket with Id {req.TicketId} is not in your cart");

            cartItem.IsDeleted = true;
            cartItem.ModifiedAtUtc = DateTime.UtcNow;

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
