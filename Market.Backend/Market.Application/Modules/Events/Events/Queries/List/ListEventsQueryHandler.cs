using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Queries.List
{
    public sealed class ListEventsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
        : IRequestHandler<ListEventsQuery, PageResult<ListEventsQueryDto>>
    {
        public async Task<PageResult<ListEventsQueryDto>> Handle(ListEventsQuery req, CancellationToken ct)
        {
            

            var q = ctx.Events.AsNoTracking();

            var searchTerm = req.Search?.ToLower().Trim() ?? string.Empty;

            if(!string.IsNullOrWhiteSpace(searchTerm))
                q = q.Where(x=>x.Name.ToLower().Contains(searchTerm));

            if(currentUser.IsOrganiser)
            {
                var org = await ctx.Organizers.Where(x => x.UserId == currentUser.UserId).FirstOrDefaultAsync(ct);
                q = q.Where(x => x.OrganizerId == org.Id);
            }    

            var projectedQuery = q.OrderBy(x => x.ScheduledDate)
                .Select(x => new ListEventsQueryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ScheduledDate = x.ScheduledDate,
                    Description = x.Description,
                    Organizer = new ListEventsQueryDtoOrganizer
                    {
                        Id = x.Organizer.Id,
                        Name = x.Organizer.Name,
                        Address = x.Organizer.Address,
                        City = x.Organizer.City.Name,
                        UserName = x.Organizer.User.UserName
                    },
                    Image = new byte[0],
                    VenueName = x.Venue.Name,
                    EventType = x.EventType.Name,
                });

            return await PageResult<ListEventsQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
