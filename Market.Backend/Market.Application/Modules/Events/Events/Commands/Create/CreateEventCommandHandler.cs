using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Commands.Create
{
    public class CreateEventCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<CreateEventCommand, int>
    {
        public async Task<int> Handle(CreateEventCommand req, CancellationToken ct)
        {
            if (!appCurrentUser.IsOrganiser)
                throw new MarketBusinessRuleException("111", "Only an organiser can add Events");

            var org = await ctx.Organizers
                .Where(x => x.UserId == appCurrentUser.UserId)
                .FirstOrDefaultAsync(ct);

            if (org == null)
                throw new MarketNotFoundException("No organizer found");

            

            if (await ctx.Venues.FirstOrDefaultAsync(x=>x.Id==req.VenueId, ct) == null)
                throw new MarketNotFoundException($"Venue with an Id of {req.VenueId} does not exist");

            

            if (await ctx.EventTypes.FirstOrDefaultAsync(x=>x.Id==req.EventTypeId, ct) == null)
                throw new MarketNotFoundException("Event type not found");

            var newEvent = new EventEntity
            {
                Name = req.Name,
                Description = req.Description,
                ScheduledDate = req.ScheduledDate,
                OrganizerId = org.Id,
                VenueId = req.VenueId,
                Image = new byte[0],
                EventTypeId = req.EventTypeId,
            };

            ctx.Events.Add(newEvent);
            await ctx.SaveChangesAsync(ct);

            var performerQ = ctx.Performers.AsNoTracking();
            foreach (var performer in req.Performers)
            {
                if (await performerQ.FirstOrDefaultAsync(x => x.Id == performer.PerformerId, ct) is null)
                    throw new MarketNotFoundException("Performer does not exist");
                var PerformerEvent = new PerformerEventEntity
                {
                    Event = newEvent,
                    PerformerId = performer.PerformerId,
                    TimeStamp = performer.TimeStamp
                };

                ctx.PerformerEvents.Add(PerformerEvent);
            }

            await ctx.SaveChangesAsync(ct);

            return newEvent.Id;
        }
    }
}
