using Market.Application.Common.Exceptions;
using Market.Application.Modules.Identity.Person.Commands.UpdateRoles;
using Market.Tests.Common;

namespace Market.Tests.PersonTests.UnitTests;

public class UpdatePersonRolesCommandHandlerTests
{
    private static UpdatePersonRolesCommandHandler CreateHandler(
        PersonTestContext ctx, FakeAppCurrentUser currentUser) => new(ctx.Db, currentUser);

    private static UpdatePersonRolesCommand PromoteToOrganiser(int id) => new()
    {
        Id = id,
        IsAdmin = false,
        IsOrganiser = true,
        IsUser = false
    };

    [Fact]
    public async Task Handle_WhenCallerIsAPlainUser_ThrowsBusinessRule()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(PersonTestContext.OwnerPersonId));

        // The whole point of the flow: roles can only ever be granted by an admin.
        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(
                new UpdatePersonRolesCommand { Id = PersonTestContext.OwnerPersonId, IsAdmin = true },
                CancellationToken.None));

        Assert.Equal("Only an admin can change roles", ex.Message);

        var person = await ctx.NewContext().Persons.FirstAsync(x => x.Id == PersonTestContext.OwnerPersonId);
        Assert.False(person.IsAdmin);
    }

    [Fact]
    public async Task Handle_WhenCallerIsAnOrganiser_ThrowsBusinessRule()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(PersonTestContext.OwnerPersonId));

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(
                new UpdatePersonRolesCommand { Id = PersonTestContext.OtherPersonId, IsAdmin = true },
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenAdminEditsTheirOwnRoles_ThrowsBusinessRule()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin(PersonTestContext.OwnerPersonId));

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(PromoteToOrganiser(PersonTestContext.OwnerPersonId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPersonIsMissing_ThrowsNotFound()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(PromoteToOrganiser(PersonTestContext.MissingId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenCallerIsAdmin_AssignsTheRoles()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        await handler.Handle(PromoteToOrganiser(PersonTestContext.OtherPersonId), CancellationToken.None);

        var person = await ctx.NewContext().Persons.FirstAsync(x => x.Id == PersonTestContext.OtherPersonId);

        Assert.True(person.IsOrganiser);
        Assert.False(person.IsUser);
        Assert.False(person.IsAdmin);
    }
}
