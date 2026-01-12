using Market.Application.Sales.Sales.Orders.Commands.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Commands.Update
{
    public class UpdateOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<UpdateOrderCommand, int>
    {
        public async Task<int> Handle(UpdateOrderCommand req, CancellationToken ct)
        {
            #region Validation

            if (await ctx.Persons.FirstOrDefaultAsync(x => x.Id == req.PersonId, ct) is null)
                throw new MarketNotFoundException("Person does not exist");
            foreach (var item in req.OrderItems)
            {
                if (await ctx.Tickets.FirstOrDefaultAsync(x => x.Id == item.TicketId, ct) is null)
                    throw new MarketNotFoundException($"Ticket  does not exist");
            }
            if (!appCurrentUser.IsAdmin)
                throw new MarketBusinessRuleException("111", "Only admins can update event details");
            
            #endregion
            #region Querying existing event and setting basic properties
            var Order = await ctx.Orders
                .Where(x => x.Id == req.Id && 
                    x.PersonId == ctx.Persons
                        .Where(x=>x.Id==appCurrentUser.UserId)
                        .First().Id)
                .FirstOrDefaultAsync(ct);

            if (Order == null)
                throw new MarketNotFoundException("This order does not exist");

            Order.PersonId = req.PersonId;
            #endregion
            #region Querying existing OrderItems
            var existingItems = await ctx.OrderItems
                .Where(x=>x.OrderId==Order.Id)
                .ToListAsync(ct);

            Dictionary<int, OrderItemEntity> existingItemsMap= existingItems.ToDictionary(x => x.Id);
            #endregion
            #region Deleting order items
            var ItemsToDelete = existingItems
                .Where(x => req.OrderItems.All(p => p.Id!= x.OrderId))
                .ToList();

            ctx.OrderItems.RemoveRange(ItemsToDelete);
            #endregion
            #region Modifying Order items
            var q = ctx.Tickets.AsNoTracking();

            var tickets = await ctx.Tickets.ToListAsync(ct);
            Dictionary<int, TicketsEntity> allTickets = tickets.ToDictionary(x => x.Id);
            //change later (assigned loyality programme to user)
            decimal loyalityProgramme = 0.05m;
            foreach (var item in req.OrderItems)
            {

                var ticketValidator = await q.FirstOrDefaultAsync(x => x.Id == item.TicketId, ct);
                if (ticketValidator is null || ticketValidator.IsDeleted)
                    throw new MarketNotFoundException("Ticket does not exist");
               
                OrderItemEntity orderItem = null;

                if (item.Id == 0)
                {
                    decimal subtotal = RoundMoney(allTickets[item.TicketId].UnitPrice * item.Quantity);
                    decimal discountAmount = RoundMoney(subtotal * loyalityProgramme);
                    decimal total = RoundMoney(subtotal - discountAmount);
                    orderItem = new OrderItemEntity
                    {
                        Order =Order,
                        TicketId=item.TicketId,
                        Quantity = item.Quantity,
                        Subtotal = subtotal,
                        DiscountAmount = discountAmount,
                        DiscountPercent = loyalityProgramme,
                        Total = total
                        
                    };

                    ctx.OrderItems.Add(orderItem);
                }
                else
                {
                    orderItem = existingItemsMap.GetValueOrDefault(item.Id);

                    if (orderItem is null)
                        throw new ValidationException("order item was not found as part of this order");
                    
                }
            }
            #endregion

            await ctx.SaveChangesAsync(ct);

            return Order.Id;
        }
        private static decimal RoundMoney(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
