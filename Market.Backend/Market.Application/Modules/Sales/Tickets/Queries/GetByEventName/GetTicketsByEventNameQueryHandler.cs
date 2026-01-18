using Market.Application.Modules.Sales.Tickets.Queries.GetByEventId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Queries.GetByEventName
{
    public sealed class GetTicketsByEventNameQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetTicketsByEventNameQuery, PageResult<GetTicketsByEventNameQueryDto>>
    {
        public async Task<PageResult<GetTicketsByEventNameQueryDto>> Handle(GetTicketsByEventNameQuery req, CancellationToken ct)
        {
            var q = ctx.Tickets.AsNoTracking();
            var searchTerm = req.EventName?.Trim().ToLower() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                q = q.Where(x => x.Event.Name.ToLower().Contains(searchTerm));
            }

            var projectedQuery = q.OrderBy(x => x.CreatedAtUtc)
                    .Select(x => new GetTicketsByEventNameQueryDto
                    {
                        Id = x.Id,
                        Event = new GetTicketsByEventNameQueryDtoEvent
                        {
                            Id = x.Event.Id,
                            Name = x.Event.Name,
                        },
                        TicketType = new GetTicketsByEventNameQueryDtoTicketType
                        {
                            Id = x.TicketType.Id,
                            Name = x.TicketType.Name,
                        },
                        QuantityInStock = x.QuantityInStock,
                        UnitPrice = x.UnitPrice,
                        Benefits = x.Benefits,
                    });
            return await PageResult<GetTicketsByEventNameQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
