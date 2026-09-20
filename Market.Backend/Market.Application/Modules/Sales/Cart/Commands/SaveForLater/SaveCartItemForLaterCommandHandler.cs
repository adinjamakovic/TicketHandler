using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Commands.SaveForLater
{
    // Parks a cart line without losing it: the row stays, only the flag moves, so the
    // quantity the person picked survives until they move it back.
    public class SaveCartItemForLaterCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<SaveCartItemForLaterCommand, Unit>
    {
        public async Task<Unit> Handle(SaveCartItemForLaterCommand req, CancellationToken ct)
        {
            int personId = CartPerson.RequireId(appCurrentUser);

            var cartItem = await ctx.CartItems
                .FirstOrDefaultAsync(x => x.PersonId == personId && x.TicketId == req.TicketId, ct);

            if (cartItem is null)
                throw new MarketNotFoundException($"Ticket with Id {req.TicketId} is not in your cart");

            // Saving something that is already saved is a no-op rather than an error —
            // two clicks on the same button shouldn't fail the second time.
            if (cartItem.IsSavedForLater)
                return Unit.Value;

            cartItem.IsSavedForLater = true;
            cartItem.ModifiedAtUtc = DateTime.UtcNow;

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
