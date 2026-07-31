namespace Market.Application.Modules.Identity.Person.Commands.Update;

public class UpdatePersonCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
    : IRequestHandler<UpdatePersonCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePersonCommand req, CancellationToken ct)
    {
        if (appCurrentUser.UserId != req.Id)
            throw new MarketBusinessRuleException("111", "This user can not edit this person");

        var person = await ctx.Persons
            .Where(x => x.Id == req.Id)
            .FirstOrDefaultAsync(ct);

        if (person is null)
            throw new MarketNotFoundException("Person not found");

        var normalizedEmail = req.Email.Trim();

        if (await ctx.Persons.AnyAsync(x => x.Id != req.Id && x.Email == normalizedEmail, ct))
            throw new MarketConflictException("Use a different email");

        if (await ctx.Cities.FirstOrDefaultAsync(x => x.Id == req.CityId, ct) is not { IsDeleted: false })
            throw new MarketNotFoundException("The selected city does not exist");

        person.FirstName = req.FirstName.Trim();
        person.LastName = req.LastName.Trim();
        person.BirthDate = req.BirthDate;
        person.CityId = req.CityId;
        person.Address = req.Address.Trim();
        person.Gender = req.Gender;
        person.Phone = req.Phone;
        person.Email = normalizedEmail;

        if (!string.IsNullOrWhiteSpace(req.Password?.Trim()))
            person.PasswordHash = new PasswordHasher<PersonEntity>().HashPassword(person, req.Password);

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
