using Market.Application.Modules.Sales.Orders.Queries.GetById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.ListWithTickets
{
    public sealed class ListOrdersWithTicketsQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListOrdersWithTicketsQuery, PageResult<ListOrdersWithTicketsQueryDto>>
    {
        public async Task<PageResult<ListOrdersWithTicketsQueryDto>> Handle(ListOrdersWithTicketsQuery req, CancellationToken ct)
        {
            var q = ctx.Orders.AsNoTracking();

            var searchTerm = req.Id;

            var projectedQuery = q.OrderBy(x => x.CreatedAtUtc)
                .Select(x => new ListOrdersWithTicketsQueryDto
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    Items = x.OrderItems.Select(y => new ListOrderOrderItemsWithTickets
                    {
                        Id = y.Id,
                        OrderId = y.OrderId,
                        Ticket = new ListOrdersQueryDtoTickets
                        {
                            Id = y.Ticket.Id,
                            EventId = y.Ticket.EventId,
                            TicketTypeId = y.Ticket.TicketTypeId,
                            QuanityInStock = y.Ticket.QuanityInStock,
                            UnitPrice = y.Ticket.UnitPrice,
                            Benefits = y.Ticket.Benefits
                        },
                        Quantity = y.Quantity,
                        Subtotal = y.Subtotal,
                        DiscountAmount = y.DiscountAmount,
                        DiscountPercent = y.DiscountPercent,
                        Total = y.Total
                    }).ToList()
                });

            return await PageResult<ListOrdersWithTicketsQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
