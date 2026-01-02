using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Commands.Update
{
    public class UpdateEventCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<UpdateEventCommand, int>
    {
        public async Task<int> Handle(UpdateEventCommand req, CancellationToken ct)
        {
            #region Validation
            if (!appCurrentUser.IsOrganiser)
                throw new MarketBusinessRuleException("111", "Only organisers can update event details");

            if (await ctx.Venues.FirstOrDefaultAsync(x => x.Id == req.VenueId, ct) is null)
                throw new MarketNotFoundException("Venue does not exist");

            if (await ctx.Venues.FirstOrDefaultAsync(x => x.Id == req.EventTypeId, ct) is null)
                throw new MarketNotFoundException("Event type does not exist");
            #endregion
            #region Querying existing event and setting basic properties
            var Event = await ctx.Events
                .Where(x => x.Id == req.Id && 
                    x.OrganizerId == ctx.Organizers
                        .Where(x=>x.UserId==appCurrentUser.UserId)
                        .First().Id)
                .FirstOrDefaultAsync(ct);

            if (Event == null)
                throw new MarketNotFoundException("This event does not exist");

            Event.Name = req.Name;
            Event.Description = req.Description;
            Event.ScheduledDate = req.ScheduledDate;
            Event.VenueId = req.VenueId;
            Event.EventTypeId = req.EventTypeId;
            #endregion
            #region Querying existing performers
            var existingPerformers = await ctx.PerformerEvents
                .Where(x=>x.EventId==Event.Id)
                .ToListAsync(ct);

            Dictionary<int, PerformerEventEntity> existingPerformersMap = existingPerformers.ToDictionary(x => x.Id);
            #endregion
            #region Deleting Event performers
            var performersToDelete = existingPerformers
                .Where(x => req.Performers.All(p => p.PerformerId != x.PerformerId))
                .ToList();

            ctx.PerformerEvents.RemoveRange(performersToDelete);
            #endregion
            #region Modifying Event Performers
            var q = ctx.Performers.AsNoTracking();
            foreach (var performer in req.Performers)
            {

                var performerValidator = await q.FirstOrDefaultAsync(x => x.Id == performer.PerformerId, ct);
                if (performerValidator is null || performerValidator.IsDeleted)
                    throw new MarketNotFoundException("Performer does not exist");
               
                PerformerEventEntity performerEvent = null;

                if (performer.Id == 0)
                {
                    performerEvent = new PerformerEventEntity
                    {
                        Event = Event,
                        PerformerId = performer.PerformerId,
                        TimeStamp = performer.TimeStamp
                    };

                    ctx.PerformerEvents.Add(performerEvent);
                }
                else
                {
                    performerEvent = existingPerformersMap.GetValueOrDefault(performer.Id);

                    if (performerEvent is null)
                        throw new ValidationException("Performer was not found as part of this event");
                    
                    performerEvent.TimeStamp = performer.TimeStamp;
                }
            }
            #endregion

            await ctx.SaveChangesAsync(ct);

            return Event.Id;
        }
    }
}
