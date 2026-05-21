namespace Market.Application.Modules.Events.Events.Commands.Update;

public class UpdateEventCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser appCurrentUser,
    IImageStorage imageStorage)
    : IRequestHandler<UpdateEventCommand, int>
{
    public async Task<int> Handle(UpdateEventCommand req, CancellationToken ct)
    {
        if (!appCurrentUser.IsOrganiser)
            throw new MarketBusinessRuleException("111", "Only organisers can update event details");

        if (await ctx.Venues.FirstOrDefaultAsync(x => x.Id == req.VenueId, ct) is null)
            throw new MarketNotFoundException("Venue does not exist");

        if (await ctx.EventTypes.FirstOrDefaultAsync(x => x.Id == req.EventTypeId, ct) is null)
            throw new MarketNotFoundException("Event type does not exist");

        var organizerId = await ctx.Organizers
            .Where(x => x.UserId == appCurrentUser.UserId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);

        var eventEntity = await ctx.Events
            .Where(x => x.Id == req.Id && x.OrganizerId == organizerId)
            .FirstOrDefaultAsync(ct);

        if (eventEntity is null)
            throw new MarketNotFoundException("This event does not exist");

        eventEntity.Name = req.Name;
        eventEntity.Description = req.Description;
        eventEntity.ScheduledDate = req.ScheduledDate;
        eventEntity.VenueId = req.VenueId;
        eventEntity.EventTypeId = req.EventTypeId;

        eventEntity.Image = await imageStorage.ReplaceIfUploadedAsync(
            ImageStorageCategory.Events, eventEntity.Image, req.Image, ct);

        var existingPerformers = await ctx.PerformerEvents
            .Where(x => x.EventId == eventEntity.Id)
            .ToListAsync(ct);

        var existingPerformersMap = existingPerformers.ToDictionary(x => x.Id);

        var performersToDelete = existingPerformers
            .Where(x => req.Performers.All(p => p.PerformerId != x.PerformerId))
            .ToList();

        ctx.PerformerEvents.RemoveRange(performersToDelete);

        var q = ctx.Performers.AsNoTracking();
        foreach (var performer in req.Performers)
        {
            var performerValidator = await q.FirstOrDefaultAsync(x => x.Id == performer.PerformerId, ct);
            if (performerValidator is null || performerValidator.IsDeleted)
                throw new MarketNotFoundException("Performer does not exist");

            if (performer.Id == 0)
            {
                ctx.PerformerEvents.Add(new PerformerEventEntity
                {
                    Event = eventEntity,
                    PerformerId = performer.PerformerId,
                    TimeStamp = performer.TimeStamp
                });
            }
            else
            {
                if (!existingPerformersMap.TryGetValue(performer.Id, out var performerEvent))
                    throw new ValidationException("Performer was not found as part of this event");

                performerEvent.TimeStamp = performer.TimeStamp;
            }
        }

        await ctx.SaveChangesAsync(ct);

        return eventEntity.Id;
    }
}
