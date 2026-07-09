namespace Market.Application.Modules.Events.Organizers.Commands.Delete;

public class DeleteOrganizerCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser appCurrentUser,
    IImageStorage imageStorage)
    : IRequestHandler<DeleteOrganizerCommand, Unit>
{
    public async Task<Unit> Handle(DeleteOrganizerCommand req, CancellationToken ct)
    {
        if (appCurrentUser.IsUser)
            throw new MarketBusinessRuleException("111", "Only an admin or organizer user can delete an organizer");

        var organizer = await ctx.Organizers
            .Where(x => x.Id == req.Id)
            .FirstOrDefaultAsync(ct);

        if (organizer is null)
            throw new MarketNotFoundException("Organizer was not found");

        var user = await ctx.Persons
            .Where(x => x.Id == organizer.UserId)
            .FirstOrDefaultAsync(ct);

        if (user is null)
            throw new MarketNotFoundException("User associated with the organizer was not found");

        await imageStorage.DeleteIfExistsAsync(ImageStorageCategory.Organizers, organizer.Logo, ct);

        ctx.Organizers.Remove(organizer);
        ctx.Persons.Remove(user);
        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
