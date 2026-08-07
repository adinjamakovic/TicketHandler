using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Commands.Clear
{
    public class ClearCartCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<ClearCartCommand, Unit>
    {
        public async Task<Unit> Handle(ClearCartCommand req, CancellationToken ct)
        {
            int personId = CartPerson.RequireId(appCurrentUser);

            var cartItems = await ctx.CartItems
                .Where(x => x.PersonId == personId)
                .ToListAsync(ct);

            if (cartItems.Count == 0)
                return Unit.Value;

            foreach (var cartItem in cartItems)
            {
                cartItem.IsDeleted = true;
                cartItem.ModifiedAtUtc = DateTime.UtcNow;
            }

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
