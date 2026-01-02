using Market.Application.Modules.Events.Organizers.Queries.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Queries.List
{
    public sealed class ListEventsWithPerformersQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListEventsWithPerformersQuery, PageResult<ListEventsWithPerformersQueryDto>>
    {
        public async Task<PageResult<ListEventsWithPerformersQueryDto>> Handle(ListEventsWithPerformersQuery req, CancellationToken ct)
        {
            var q = ctx.Events.AsNoTracking();

            var searchTerm = req.Search?.ToLower().Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(searchTerm))
                q = q.Where(x => x.Name.ToLower().Contains(searchTerm));

            var projectedQuery = q.OrderBy(x => x.ScheduledDate)
                .Select(x => new ListEventsWithPerformersQueryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ScheduledDate = x.ScheduledDate,
                    OrganizerName = x.Organizer.Name,
                    VenueName = x.Venue.Name,
                    Image = new byte[0],
                    EventTypeName = x.EventType.Name,
                    Performers = x.PerformerEvents
                   .Select(y => new ListEventsQueryDtoPerformers
                   {
                       Name = y.Performer.Name,
                       Description = y.Performer.Description,
                       Genre = y.Performer.Genre.Name
                   }).ToList()
                });

            return await PageResult<ListEventsWithPerformersQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
