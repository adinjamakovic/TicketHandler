namespace Market.Application.Modules.Events.Organizers.Commands.Update;

public class UpdateOrganizerCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser appCurrentUser,
    IImageStorage imageStorage)
    : IRequestHandler<UpdateOrganizerCommand, Unit>
{
    public async Task<Unit> Handle(UpdateOrganizerCommand req, CancellationToken ct)
    {
        if (appCurrentUser.IsUser)
            throw new MarketBusinessRuleException("111", "User can not edit organisers... How did we get here??");

        if ((await ctx.Organizers.Where(x => x.UserId == appCurrentUser.UserId).FirstOrDefaultAsync(ct))?.Id != req.Id
            && appCurrentUser.IsAdmin == false)
            throw new MarketBusinessRuleException("111", "This user cant edit this organizer");

        var entity = await ctx.Organizers
            .Where(x => x.Id == req.Id)
            .FirstOrDefaultAsync(ct);

        if (entity is null)
            throw new MarketNotFoundException("Invalid Organizer");

        var user = await ctx.Persons
            .Where(x => x.Id == entity.UserId)
            .FirstOrDefaultAsync(ct);

        if (user is null)
            throw new MarketNotFoundException("This organizer doesn't have a user");

        if (await ctx.Organizers.AnyAsync(x => x.Id != req.Id && x.Name.ToLower() == req.Name.ToLower(), ct))
            throw new MarketConflictException("Name already exists");

        if (await ctx.Cities.FirstOrDefaultAsync(x => x.Id == req.CityId, ct) is not { IsDeleted: false })
            throw new MarketNotFoundException("The selected city does not exist");

        if (await ctx.Cities.FirstOrDefaultAsync(x => x.Id == req.User.CityId, ct) is not { IsDeleted: false })
            throw new MarketNotFoundException("The selected city for the user does not exist");

        entity.Name = req.Name.Trim();
        entity.Description = req.Description?.Trim();
        entity.Address = req.Address.Trim();
        entity.CityId = req.CityId;
        entity.Logo = await imageStorage.ReplaceIfUploadedAsync(
            ImageStorageCategory.Organizers, entity.Logo, req.Logo, ct);


        user.FirstName = req.User.FirstName.Trim();
        user.LastName = req.User.LastName.Trim();
        user.BirthDate = req.User.BirthDate;
        user.Email = req.User.Email;
        user.Address = req.User.Address.Trim();
        user.CityId = req.CityId;
        user.Gender = req.User.Gender;
        user.Phone = req.User.Phone;

        if (!string.IsNullOrWhiteSpace(req.User.Password?.Trim()))
            user.PasswordHash = new PasswordHasher<PersonEntity>().HashPassword(user, req.User.Password);

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
