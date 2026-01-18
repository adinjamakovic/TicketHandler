using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.GetById
{
    public sealed class GetOrderByIdQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetOrderByIdQuery, GetOrderByIdQueryDto>
    {
        public async Task<GetOrderByIdQueryDto> Handle(GetOrderByIdQuery req, CancellationToken ct)
        {
            var q = ctx.Orders
                .Where(x => x.Id == req.Id);

            var dto = await q.Select(x => new GetOrderByIdQueryDto
            {
                Id = x.Id,
                PersonId = x.PersonId,
                Items = x.OrderItems
                .Select(y => new GetOrderByIdOrderItems
                {
                    Id= y.Id,
                    OrderId = y.OrderId,
                    Ticket = new GetOrderByIdQueryDtoTicket
                    {
                        Id = y.Ticket.Id,
                        EventId = y.Ticket.EventId,
                        TicketTypeId= y.Ticket.TicketTypeId,
                        QuanityInStock= y.Ticket.QuantityInStock,
                        UnitPrice= y.Ticket.UnitPrice,
                        Benefits= y.Ticket.Benefits
                    },
                    Quantity = y.Quantity,
                    Subtotal = y.Subtotal,
                    DiscountAmount = y.DiscountAmount,
                    DiscountPercent = y.DiscountPercent,
                    Total = y.Total,
                }).ToList()
            }).FirstOrDefaultAsync(ct);

            if (dto is null)
                throw new MarketNotFoundException($"Order with Id {req.Id} not found");
            return dto;
        }
    }
}
