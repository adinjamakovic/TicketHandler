namespace Market.Application.Modules.Events.Events.Commands.Delete;

public class DeleteEventCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser appCurrent,
    IImageStorage imageStorage)
    : IRequestHandler<DeleteEventCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEventCommand req, CancellationToken ct)
    {
        if (!appCurrent.IsAdmin && !appCurrent.IsOrganiser)
            throw new MarketBusinessRuleException("111", "Only an admin or an organizer can delete an event");

        var eventEntity = await ctx.Events
            .FirstOrDefaultAsync(x => x.Id == req.Id, ct);

        if (eventEntity is null)
            throw new MarketNotFoundException($"Event with Id {req.Id} does not exist");

        // Admins may remove any event; an organizer only the events they own.
        if (!appCurrent.IsAdmin)
        {
            var organizerId = await ctx.Organizers
                .Where(x => x.UserId == appCurrent.UserId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(ct);

            if (organizerId is null)
                throw new MarketNotFoundException("No organizer found");

            if (eventEntity.OrganizerId != organizerId)
                throw new MarketBusinessRuleException("111", $"Event with Id {req.Id} belongs to another organizer");
        }

        var performerEvents = await ctx.PerformerEvents
            .Where(x => x.EventId == req.Id)
            .ToListAsync(ct);

        ctx.PerformerEvents.RemoveRange(performerEvents);
        ctx.Events.Remove(eventEntity);

        await ctx.SaveChangesAsync(ct);

        // Dropped only once the event is gone: a failed delete must not strip the poster off an
        // event that is still there.
        await imageStorage.DeleteIfExistsAsync(ImageStorageCategory.Events, eventEntity.Image, ct);

        return Unit.Value;
    }
}
