using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.GetById
{
    public class GetOrganizerByIdQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetOrganizerByIdQuery, GetOrganizerByIdQueryDto>
    {
        public async Task<GetOrganizerByIdQueryDto> Handle(GetOrganizerByIdQuery req, CancellationToken ct)
        {
            var Organizer = await ctx.Organizers
                .Where(x => x.Id == req.Id)
                .Select(x => new GetOrganizerByIdQueryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    City = x.City.Name,
                    CityId = x.City.Id,
                    Address = x.Address,
                    User = new GetOrganizerByIdQueryDtoUser
                    {
                        FirstName = x.User.FirstName,
                        LastName = x.User.LastName,
                        BirthDate = x.User.BirthDate,
                        CityId = x.User.CityId,
                        Address = x.User.Address,
                        Gender = x.User.Gender,
                        Phone = x.User.Phone,
                        Email = x.User.Email
                    },
                    Events = x.Events.Select(y => new GetOrganizerByIdQueryEventDto
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Description = y.Description,
                        ScheduledDate = y.ScheduledDate
                    }).ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (Organizer == null)
                throw new MarketNotFoundException($"Organizer with an Id of {req.Id} was not found");
            
            return Organizer;
        }
    }
}
