using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.GetByUserId
{
    public class GetOrganizerByUserIdQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetOrganizerByUserIdQuery, GetOrganizerByUserIdQueryDto>
    {
        public async Task<GetOrganizerByUserIdQueryDto> Handle(GetOrganizerByUserIdQuery req, CancellationToken ct)
        {
            var Organizer = await ctx.Organizers
                .Where(x => x.UserId == req.UserId)
                .Select(x => new GetOrganizerByUserIdQueryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Email = x.User.Email,
                    City = x.City.Name,
                    Events = x.Events.Select(y => new GetOrganizerByUserIdQueryEventDto
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Description = y.Description,
                        ScheduledDate = y.ScheduledDate
                    }).ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (Organizer == null)
                throw new MarketNotFoundException($"Organizer with an Id of {req.UserId} was not found");
            
            return Organizer;
        }
    }
}
