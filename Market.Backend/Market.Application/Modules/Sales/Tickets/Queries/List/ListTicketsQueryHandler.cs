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

            if (req.EventId is not null)
                q = q.Where(x => x.EventId == req.EventId);

            var projectedQ = q.OrderBy(x => x.CreatedAtUtc)
                .Select(x => new ListTicketsQueryDto
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    TicketTypeId = x.TicketTypeId,
                    QuanityInStock = x.QuanityInStock,
                    UnitPrice = x.UnitPrice,
                    Benefits = x.Benefits
                });

            return await PageResult<ListTicketsQueryDto>.FromQueryableAsync(projectedQ, req.Paging, ct);
        }
    }
}
