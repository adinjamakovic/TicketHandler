using Market.Application.Common.Exceptions;
using Market.Application.Modules.Events.Organizers.Commands.Delete;
using Market.Tests.Common;

namespace Market.Tests.OrganizerTests.UnitTests;

public class DeleteOrganizerCommandHandlerTests
{
    private static DeleteOrganizerCommandHandler CreateHandler(
        OrganizersTestContext ctx,
        FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser, ctx.ImageStorage);

    [Fact]
    public async Task Handle_WhenCallerIsPlainUser_ThrowsBusinessRule()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        var organizer = await ctx.AddOrganizerAsync(OrganizersTestContext.OrganizerUserId);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User());

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(new DeleteOrganizerCommand { Id = organizer.Id }, CancellationToken.None));

        Assert.Equal("Only an admin or organizer user can delete an organizer", ex.Message);
        Assert.NotNull(await ctx.GetOrganizerAsync(organizer.Id));
    }

    [Fact]
    public async Task Handle_WhenOrganizerDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(
                new DeleteOrganizerCommand { Id = OrganizersTestContext.MissingId },
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenOrganizerDeletesAnotherOrganizer_ThrowsBusinessRule()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        var target = await ctx.AddOrganizerAsync(OrganizersTestContext.OrganizerUserId, "Sarajevo Events");
        await ctx.AddOrganizerAsync(OrganizersTestContext.OtherOrganizerUserId, "Mostar Events", OrganizersTestContext.MostarCityId);

        // Organizer B signs in and aims at organizer A's record.
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(OrganizersTestContext.OtherOrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(new DeleteOrganizerCommand { Id = target.Id }, CancellationToken.None));

        Assert.Equal("An organizer can only delete their own record", ex.Message);
        Assert.NotNull(await ctx.GetOrganizerAsync(target.Id));
        Assert.Equal(2, (await ctx.GetOrganizersAsync()).Count);
        Assert.Empty(ctx.ImageStorage.Deleted);
    }

    [Fact]
    public async Task Handle_WhenOrganizerDeletesAnotherOrganizer_LeavesThatOrganizersPersonIntact()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        var target = await ctx.AddOrganizerAsync(OrganizersTestContext.OrganizerUserId);
        await ctx.AddOrganizerAsync(OrganizersTestContext.OtherOrganizerUserId, "Mostar Events", OrganizersTestContext.MostarCityId);

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(OrganizersTestContext.OtherOrganizerUserId));

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(new DeleteOrganizerCommand { Id = target.Id }, CancellationToken.None));

        Assert.Equal(2, await ctx.CountPersonsAsync());
    }

    [Fact]
    public async Task Handle_WhenOrganizerDeletesOwnRecord_SoftDeletesOrganizerAndPerson()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        var organizer = await ctx.AddOrganizerAsync(OrganizersTestContext.OrganizerUserId, logo: "organizers/logo.png");
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(OrganizersTestContext.OrganizerUserId));

        await handler.Handle(new DeleteOrganizerCommand { Id = organizer.Id }, CancellationToken.None);

        Assert.Null(await ctx.GetOrganizerAsync(organizer.Id));
        Assert.Equal(0, await ctx.CountPersonsAsync());
        Assert.Equal("organizers/logo.png", Assert.Single(ctx.ImageStorage.Deleted));
    }

    [Fact]
    public async Task Handle_WhenCallerIsAdmin_DeletesAnyOrganizer()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        var organizer = await ctx.AddOrganizerAsync(OrganizersTestContext.OrganizerUserId);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        await handler.Handle(new DeleteOrganizerCommand { Id = organizer.Id }, CancellationToken.None);

        Assert.Null(await ctx.GetOrganizerAsync(organizer.Id));
    }
}
