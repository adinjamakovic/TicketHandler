using Market.Application.Modules.Sales.Orders.Queries.GetById;
using Market.Application.Modules.Sales.Orders.Queries.ListWithTicketsAndEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.ListWithTicketsAndEvents
{
    public sealed class ListOrdersWithTicketsAndEventsQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListOrdersWithTicketsAndEventsQuery, PageResult<ListOrdersWithTicketsAndEventsQueryDto>>
    {
        public async Task<PageResult<ListOrdersWithTicketsAndEventsQueryDto>> Handle(ListOrdersWithTicketsAndEventsQuery req, CancellationToken ct)
        {
            var q = ctx.Orders.AsNoTracking();

            var searchTerm = req.Id;

            var projectedQuery = q.OrderBy(x => x.CreatedAtUtc)
                .Select(x => new ListOrdersWithTicketsAndEventsQueryDto
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    Items = x.OrderItems.Select(y => new ListOrderOrderItemsWithTicketsAndEvents
                    {
                        Id = y.Id,
                        OrderId = y.OrderId,
                        Ticket = new ListOrdersWithEventsQueryDtoTickets
                        {
                            Id = y.Ticket.Id,
                            Event = new ListOrdersQueryDtoEvent
                            {
                                Name = y.Ticket.Event.Name,
                                Description = y.Ticket.Event.Description,
                                ScheduledDate = y.Ticket.Event.ScheduledDate,
                                OrganizerId = y.Ticket.Event.OrganizerId,
                                VenueId = y.Ticket.Event.VenueId,
                                EventTypeId = y.Ticket.Event.EventTypeId
                            },
                            TicketTypeId = y.Ticket.TicketTypeId,
                            QuanityInStock = y.Ticket.QuantityInStock,
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

            return await PageResult<ListOrdersWithTicketsAndEventsQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
