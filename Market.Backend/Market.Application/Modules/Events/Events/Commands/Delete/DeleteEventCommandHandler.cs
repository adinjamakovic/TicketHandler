namespace Market.Application.Modules.Events.Events.Commands.Delete;

public class DeleteEventCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser appCurrent,
    IImageStorage imageStorage)
    : IRequestHandler<DeleteEventCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEventCommand req, CancellationToken ct)
    {
        if (appCurrent.IsUser)
            throw new MarketBusinessRuleException("111", "A user can not issue a delete statement on an event... how did we get here?");

        var eventEntity = await ctx.Events
            .FirstOrDefaultAsync(x => x.Id == req.Id, ct);

        if (eventEntity is null)
            throw new MarketNotFoundException($"Event with Id {req.Id} does not exist");

        imageStorage.DeleteIfExists(ImageStorageCategory.Events, eventEntity.Image);

        var performerEvents = await ctx.PerformerEvents
            .Where(x => x.EventId == req.Id)
            .ToListAsync(ct);

        ctx.PerformerEvents.RemoveRange(performerEvents);
        ctx.Events.Remove(eventEntity);

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
