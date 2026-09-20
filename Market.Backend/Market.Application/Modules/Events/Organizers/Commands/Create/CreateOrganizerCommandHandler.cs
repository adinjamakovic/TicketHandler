namespace Market.Application.Modules.Events.Organizers.Commands.Create;

public class CreateOrganizerCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser appCurrentUser,
    IImageStorage imageStorage)
    : IRequestHandler<CreateOrganizerCommand, int>
{
    public async Task<int> Handle(CreateOrganizerCommand req, CancellationToken ct)
    {
        if (!appCurrentUser.IsAdmin)
            throw new MarketBusinessRuleException("111", "Only an admin can add an organizer");

        var normalized = req.Name?.Trim();

        if (await ctx.Organizers.AnyAsync(x => x.Name == normalized, ct))
            throw new MarketConflictException("This organizer already exists");

        if (await ctx.Cities.FirstOrDefaultAsync(x => x.Id == req.CityId, ct) is null)
            throw new ValidationException("Invalid city");

        var hasher = new PasswordHasher<PersonEntity>();

        var user = new PersonEntity
        {
            FirstName = req.User.FirstName,
            LastName = req.User.LastName,
            BirthDate = req.User.BirthDate,
            CityId = req.User.CityId,
            Address = req.User.Address,
            Gender = req.User.Gender,
            Phone = req.User.Phone,
            Email = req.User.Email,
            PasswordHash = hasher.HashPassword(null!, req.User.Password),
            IsAdmin = false,
            IsUser = false,
            IsOrganiser = true,
            IsEnabled = true
        };

        ctx.Persons.Add(user);
        
        var logoPath = await imageStorage.SaveAsync(ImageStorageCategory.Organizers, req.Logo, ct);

        var organizer = new OrganizerEntity
        {
            Name = normalized!,
            Description = req.Description?.Trim(),
            Address = req.Address.Trim(),
            CityId = req.CityId,
            Logo = logoPath,
            User = user,
            IsDeleted = false,
            CreatedAtUtc = DateTime.UtcNow
        };


        try
        {
            ctx.Organizers.Add(organizer);
            await ctx.SaveChangesAsync(ct);
        }
        catch
        {
            // The logo is already uploaded, so drop it instead of leaving it orphaned,
            // then let the original failure reach MarketExceptionHandler - the caller
            // must never get a success response for an organizer that was not saved.
            try
            {
                // Not ct: cleanup still has to run when the request was cancelled.
                await imageStorage.DeleteIfExistsAsync(
                    ImageStorageCategory.Organizers, logoPath, CancellationToken.None);
            }
            catch
            {
                // A failed cleanup must not hide the failure we are about to rethrow.
            }

            throw;
        }

        return organizer.Id;
    }
}
