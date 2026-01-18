using Market.Application.Modules.Events.Events.Queries.List;
using Market.Application.Modules.Sales.Orders.Queries.GetById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.GetByPersonId
{
    public sealed class GetEventsByPersonIdQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetOrderByPersonIdQuery, PageResult<GetOrderByPersonIdQueryDto>>
    {
        public async Task<PageResult<GetOrderByPersonIdQueryDto>> Handle(GetOrderByPersonIdQuery req, CancellationToken ct)
        {
            var q = ctx.Orders.AsNoTracking();
            q = q.Where(x => x.PersonId == req.PersonId);

            var projectedQuery = q.OrderBy(x => x.CreatedAtUtc)
                .Select(x => new GetOrderByPersonIdQueryDto
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    Items = x.OrderItems
                .Select(y => new GetOrderByPersonIdOrderItems
                {
                    Id = y.Id,
                    OrderId = y.OrderId,
                    Ticket = new GetOrderByPersonIdQueryDtoTicket
                    {
                        Id = y.Ticket.Id,
                        EventId = y.Ticket.EventId,
                        TicketTypeId = y.Ticket.TicketTypeId,
                        QuanityInStock = y.Ticket.QuantityInStock,
                        UnitPrice = y.Ticket.UnitPrice,
                        Benefits = y.Ticket.Benefits
                    },
                    Quantity = y.Quantity,
                    Subtotal = y.Subtotal,
                    DiscountAmount = y.DiscountAmount,
                    DiscountPercent = y.DiscountPercent,
                    Total = y.Total,
                }).ToList()
                });

            return await PageResult<GetOrderByPersonIdQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}

