namespace Market.Application.Modules.Events.Events.Commands.Create;

public class CreateEventCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser appCurrentUser,
    IImageStorage imageStorage)
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

        if (await ctx.Venues.FirstOrDefaultAsync(x => x.Id == req.VenueId, ct) == null)
            throw new MarketNotFoundException($"Venue with an Id of {req.VenueId} does not exist");

        if (await ctx.EventTypes.FirstOrDefaultAsync(x => x.Id == req.EventTypeId, ct) == null)
            throw new MarketNotFoundException("Event type not found");

        var performerQ = ctx.Performers.AsNoTracking();
        foreach (var performer in req.Performers)
        {
            if (await performerQ.FirstOrDefaultAsync(x => x.Id == performer.PerformerId, ct) is null)
                throw new MarketNotFoundException("Performer does not exist");
        }

        var imagePath = await imageStorage.SaveAsync(ImageStorageCategory.Events, req.Image, ct);

        var newEvent = new EventEntity
        {
            Name = req.Name,
            Description = req.Description,
            ScheduledDate = req.ScheduledDate,
            OrganizerId = org.Id,
            VenueId = req.VenueId,
            Image = imagePath,
            EventTypeId = req.EventTypeId,
        };

        ctx.Events.Add(newEvent);

        foreach (var performer in req.Performers)
        {
            ctx.PerformerEvents.Add(new PerformerEventEntity
            {
                Event = newEvent,
                PerformerId = performer.PerformerId,
                TimeStamp = performer.TimeStamp
            });
        }

        try
        {
            // The event and its line-up go in as one save, so a failure leaves no half-created event.
            await ctx.SaveChangesAsync(ct);
        }
        catch
        {
            // The poster is already uploaded, so drop it instead of leaving it orphaned,
            // then let the original failure reach MarketExceptionHandler.
            try
            {
                // Not ct: cleanup still has to run when the request was cancelled.
                await imageStorage.DeleteIfExistsAsync(
                    ImageStorageCategory.Events, imagePath, CancellationToken.None);
            }
            catch
            {
                // A failed cleanup must not hide the failure we are about to rethrow.
            }

            throw;
        }

        return newEvent.Id;
    }
}
