using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Queries.List
{
    public class ListTicketsQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrent)
        : IRequestHandler<ListTicketsQuery, PageResult<ListTicketsQueryDto>>
    {
        public async Task<PageResult<ListTicketsQueryDto>> Handle(ListTicketsQuery req, CancellationToken ct)
        {
            var q = ctx.Tickets.AsNoTracking();

            var searchTerm = req.EventName?.Trim().ToLower() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                q = q.Where(x => x.Event.Name.ToLower().Contains(searchTerm));
            }

            var projectedQ = q.OrderBy(x => x.CreatedAtUtc)
                .Select(x => new ListTicketsQueryDto
                {
                    Id = x.Id,
                    Event = new ListTicketsQueryEvent
                    {
                        Id = x.EventId,
                        Name = x.Event.Name,
                    },
                    TicketType = new ListTicketsQueryTicketType
                    {
                        Id = x.TicketType.Id,
                        Name = x.TicketType.Name,
                    },
                    QuantityInStock = x.QuantityInStock,
                    UnitPrice = x.UnitPrice,
                    Benefits = x.Benefits
                });

            return await PageResult<ListTicketsQueryDto>.FromQueryableAsync(projectedQ, req.Paging, ct);
        }
    }
}
