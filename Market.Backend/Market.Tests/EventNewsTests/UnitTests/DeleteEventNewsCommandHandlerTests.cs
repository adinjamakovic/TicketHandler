using MediatR;
using Market.Application.Common.Exceptions;
using Market.Application.Modules.Events.EventsNews.Commands.Delete;
using Market.Tests.Common;

namespace Market.Tests.EventNewsTests.UnitTests;

public class DeleteEventNewsCommandHandlerTests
{
    private static DeleteEventNewsCommandHandler CreateHandler(EventNewsTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser, ctx.ImageStorage);

    [Fact]
    public async Task Handle_WhenNewsDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(new DeleteEventNewsCommand { Id = EventNewsTestContext.MissingId }, CancellationToken.None));

        Assert.Equal("Event News not found", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCallerIsPlainUser_ThrowsBusinessRule()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User());

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(new DeleteEventNewsCommand { Id = seeded.Id }, CancellationToken.None));

        Assert.Equal("You are not allowed to do this", ex.Message);
        Assert.NotNull(await ctx.GetEventNewsAsync(seeded.Id));
    }

    [Fact]
    public async Task Handle_WhenNewsBelongsToAnotherOrganiser_ThrowsBusinessRule()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OtherOrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(
                new DeleteEventNewsCommand { Id = EventNewsTestContext.RockNightNewsId },
                CancellationToken.None));

        Assert.Equal("You are not allowed to do this", ex.Message);
        Assert.NotNull(await ctx.GetEventNewsAsync(EventNewsTestContext.RockNightNewsId));
    }

    [Fact]
    public async Task Handle_WithOwnNews_SoftDeletesInsteadOfRemovingTheRow()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        var result = await handler.Handle(new DeleteEventNewsCommand { Id = seeded.Id }, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        Assert.Null(await ctx.GetEventNewsAsync(seeded.Id));

        await using var readContext = ctx.NewContext();
        var stored = await readContext.EventNews
            .IgnoreQueryFilters()
            .AsNoTracking()
            .SingleAsync(x => x.Id == seeded.Id);

        Assert.True(stored.IsDeleted);
    }

    [Fact]
    public async Task Handle_WithOwnNews_LeavesTheOtherNewsOnTheEventAlone()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        await handler.Handle(
            new DeleteEventNewsCommand { Id = EventNewsTestContext.RockNightNewsId },
            CancellationToken.None);

        var remaining = await ctx.GetEventNewsForEventAsync(EventNewsTestContext.RockNightEventId);

        Assert.Equal(EventNewsTestContext.RockNightSecondNewsId, Assert.Single(remaining).Id);
    }

    [Fact]
    public async Task Handle_WhenNewsWasAlreadyDeleted_ThrowsNotFound()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        await handler.Handle(new DeleteEventNewsCommand { Id = seeded.Id }, CancellationToken.None);
        ctx.Db.ChangeTracker.Clear();

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(new DeleteEventNewsCommand { Id = seeded.Id }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenNewsHasNoImage_DeletesNothingFromStorage()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        await handler.Handle(new DeleteEventNewsCommand { Id = seeded.Id }, CancellationToken.None);

        Assert.Empty(ctx.ImageStorage.Deleted);
    }
}
