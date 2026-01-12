using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Commands.Create
{
    internal class CreateOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
        : IRequestHandler<CreateOrderCommand, int>
    {
        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
           
            var order = new OrderEntity {
               PersonId = currentUser.UserId!.Value,
               CreatedAtUtc = DateTime.UtcNow 
           };
            ctx.Orders.Add(order);


            List<int> productIds = request.OrderItems.Select(x=>x.TicketId).ToList();
            
            List<TicketsEntity> tickets = await ctx.Tickets.Where(x=>productIds.Contains(x.Id)).AsNoTracking().ToListAsync();

            Dictionary<int, TicketsEntity> ticketsMap = tickets.ToDictionary(x => x.Id);

            decimal discountPercent = 0.05m;

            foreach (var item in request.OrderItems)
            {
                TicketsEntity? ticket = ticketsMap.GetValueOrDefault(item.TicketId);

                if (ticket is null)
                {
                    throw new ValidationException(message: $"Invalid ticketId {item.TicketId}.");
                }

                if (ticket.IsDeleted == true)
                {
                    throw new ValidationException($"Product {ticket.Event.Name} is disabled.");
                }

                decimal subtotal = RoundMoney(ticket.UnitPrice * item.Quantity);
                decimal discountAmount = RoundMoney(subtotal * discountPercent);
                decimal total = RoundMoney(subtotal - discountAmount);

                var orderItem = new OrderItemEntity
                {
                    Order = order,
                    TicketId = item.TicketId,
                    Quantity = item.Quantity,
                    Subtotal = subtotal,
                    DiscountPercent = discountPercent,
                    DiscountAmount = discountAmount,
                    Total = total
                };

                ctx.OrderItems.Add(orderItem);
            }

            await ctx.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
        private static decimal RoundMoney(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
