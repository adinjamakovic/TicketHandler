using Market.Application.Abstractions;
using Market.Application.Modules.Sales.Orders.Commands.Delete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Sales.Sales.Orders.Commands.Delete
{
    public class DeleteOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrent)
        : IRequestHandler<DeleteOrderCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteOrderCommand req, CancellationToken ct)
        {
            var Order = await ctx.Orders
                .FirstOrDefaultAsync(x => x.Id == req.Id, ct);
            var wallet  = await ctx.Wallets.FirstOrDefaultAsync(x=> x.PersonId == Order.PersonId);

            if (Order is null)
                throw new MarketNotFoundException($"Order with Id {req.Id} does not exist");

            ctx.Orders.Remove(Order);

            var OrderItems = await ctx.OrderItems
                .Where(x => x.OrderId == req.Id)
                .ToListAsync(ct);

            if(OrderItems != null)
            {
                foreach (var orderItems in OrderItems)
                {
                    wallet!.Balance += orderItems.Subtotal;
                    ctx.OrderItems.Remove(orderItems);
                }
                ctx.Wallets.Update(wallet!);
            }

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
