using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Queries.List
{
    public class ListEventNewsQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrent)
        : IRequestHandler<ListEventNewsQuery, PageResult<ListEventNewsQueryDto>>
    {
        public async Task<PageResult<ListEventNewsQueryDto>> Handle(ListEventNewsQuery req, CancellationToken ct)
        {
            var q = ctx.EventNews.AsNoTracking();

            if (appCurrent.IsOrganiser)
                req.OrganizerId = (await ctx.Organizers
                    .Where(x => x.UserId == appCurrent.UserId)
                    .FirstOrDefaultAsync(ct))?.Id;

            if (req.OrganizerId is not null)
                q = q.Where(x => x.OrganizerId == req.OrganizerId);

            if (req.EventId is not null)
                q = q.Where(x => x.EventId == req.EventId);

            var projectedQ = q.OrderBy(x => x.CreatedAtUtc)
                .Select(x => new ListEventNewsQueryDto
                {
                    Id = x.Id,
                    Event = x.Event.Name,
                    Organizer = x.Organizer.Name,
                    Header = x.Header,
                    Body = x.Body ?? string.Empty,
                    Image = new byte[0]
                });

            return await PageResult<ListEventNewsQueryDto>.FromQueryableAsync(projectedQ, req.Paging, ct);
        }
    }
}
