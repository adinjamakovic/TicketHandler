using Market.Application.Modules.Events.Organizers.Queries.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Queries.GetByEventId
{
    public sealed class GetEventsByOrganizerIdQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetTicketsByEventIdQuery, PageResult<GetTicketsByEventIdQueryDto>>
    {
        public async Task<PageResult<GetTicketsByEventIdQueryDto>> Handle(GetTicketsByEventIdQuery req, CancellationToken ct)
        {
            var q = ctx.Tickets.AsNoTracking();
            q = q.Where(x => x.EventId==req.Id);

            var projectedQuery = q.OrderBy(x => x.CreatedAtUtc)
                .Select(x => new GetTicketsByEventIdQueryDto
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    TicketTypeId = x.TicketTypeId,
                    QuanityInStock = x.QuanityInStock,
                    UnitPrice = x.UnitPrice,
                    Benefits = x.Benefits,
                });

            return await PageResult<GetTicketsByEventIdQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
