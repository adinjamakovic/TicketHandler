namespace Market.Application.Modules.Identity.Person.Commands.UpdateRoles;

public class UpdatePersonRolesCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
    : IRequestHandler<UpdatePersonRolesCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePersonRolesCommand req, CancellationToken ct)
    {
        if (!appCurrentUser.IsAdmin)
            throw new MarketBusinessRuleException("111", "Only an admin can change roles");

        if (appCurrentUser.UserId == req.Id)
            throw new MarketBusinessRuleException("111", "An admin can not change their own roles");

        var person = await ctx.Persons
            .Where(x => x.Id == req.Id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);

        if (person is null)
            throw new MarketNotFoundException("Person not found");

        person.IsAdmin = req.IsAdmin;
        person.IsOrganiser = req.IsOrganiser;
        person.IsUser = req.IsUser;

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
