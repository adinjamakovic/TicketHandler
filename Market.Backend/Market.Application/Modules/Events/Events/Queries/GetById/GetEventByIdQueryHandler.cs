using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Queries.GetById
{
    public sealed class GetEventByIdQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetEventByIdQuery, GetEventByIdQueryDto>
    {
        public async Task<GetEventByIdQueryDto> Handle(GetEventByIdQuery req, CancellationToken ct)
        {
            var q = ctx.Events
                .Where(x => x.Id == req.Id);

            
            var dto = await q.Select(x => new GetEventByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ScheduledDate = x.ScheduledDate,
                OrganizerName = x.Organizer.Name,
                VenueName = x.Venue.Name,
                Image = new byte[0],//this needs to be fixed later
                EventTypeName = x.EventType.Name,
                Performers = x.PerformerEvents
                .Select(y => new GetEventByIdQueryDtoPerformers
                {
                    Id = y.Performer.Id,
                    Name = y.Performer.Name,
                    Description = y.Performer.Description,
                    Genre = y.Performer.Genre.Name,
                }).ToList()
            }).FirstOrDefaultAsync(ct);

            if (dto is null)
                throw new MarketNotFoundException($"Event with Id {req.Id} not found");
            return dto;
        }
    }
}
