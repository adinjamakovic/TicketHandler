using Market.Application.Modules.Events.Organizers.Queries.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Queries.List
{
    public sealed class GetEventsByOrganizerIdQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetEventsByOrganizerIdQuery, PageResult<GetEventsByOrganizerIdQueryDto>>
    {
        public async Task<PageResult<GetEventsByOrganizerIdQueryDto>> Handle(GetEventsByOrganizerIdQuery req, CancellationToken ct)
        {
            var q = ctx.Events.AsNoTracking();
            q = q.Where(x => x.OrganizerId==req.Id);

            var projectedQuery = q.OrderBy(x => x.ScheduledDate)
                .Select(x => new GetEventsByOrganizerIdQueryDto
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
                   .Select(y => new GetEventsByOrganizerIdQueryDtoPerformers
                   {
                       Name = y.Performer.Name,
                       Description = y.Performer.Description,
                       Genre = y.Performer.Genre.Name
                   }).ToList()
                });

            return await PageResult<GetEventsByOrganizerIdQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
