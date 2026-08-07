using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Commands.AddItem
{
    public class AddCartItemCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<AddCartItemCommand, Unit>
    {
        public async Task<Unit> Handle(AddCartItemCommand req, CancellationToken ct)
        {
            int personId = CartPerson.RequireId(appCurrentUser);

            var ticket = await ctx.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == req.TicketId, ct);

            if (ticket is null)
                throw new MarketNotFoundException($"Ticket with Id {req.TicketId} does not exist");

            var cartItem = await ctx.CartItems
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.PersonId == personId && x.TicketId == req.TicketId, ct);

            decimal alreadyInCart = cartItem is null || cartItem.IsDeleted ? 0 : cartItem.Quantity;
            decimal requestedTotal = alreadyInCart + req.Quantity;

            if (requestedTotal > ticket.QuantityInStock)
                throw new MarketBusinessRuleException(
                    "CART_OUT_OF_STOCK",
                    $"Only {ticket.QuantityInStock} ticket(s) left, you already have {alreadyInCart} in your cart.");

            if (cartItem is null)
            {
                ctx.CartItems.Add(new CartItemEntity
                {
                    PersonId = personId,
                    TicketId = req.TicketId,
                    Quantity = req.Quantity,
                    CreatedAtUtc = DateTime.UtcNow,
                    ModifiedAtUtc = DateTime.UtcNow
                });
            }
            else
            {
                if (cartItem.IsDeleted)
                {
                    cartItem.IsDeleted = false;
                    cartItem.CreatedAtUtc = DateTime.UtcNow;
                }

                cartItem.Quantity = requestedTotal;
                cartItem.ModifiedAtUtc = DateTime.UtcNow;
            }

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
