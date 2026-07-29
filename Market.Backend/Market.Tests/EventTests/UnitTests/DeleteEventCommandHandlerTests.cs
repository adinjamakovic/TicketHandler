using MediatR;
using Market.Application.Common.Exceptions;
using Market.Application.Modules.Events.Events.Commands.Delete;
using Market.Tests.Common;

namespace Market.Tests.EventTests.UnitTests;

public class DeleteEventCommandHandlerTests
{
    private static DeleteEventCommandHandler CreateHandler(EventsTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser, ctx.ImageStorage);

    [Fact]
    public async Task Handle_WhenCallerIsPlainUser_ThrowsBusinessRule()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User());

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(new DeleteEventCommand { Id = seeded.Id }, CancellationToken.None));

        await using var readContext = ctx.NewContext();
        Assert.True(await readContext.Events.AnyAsync(x => x.Id == seeded.Id));
    }

    [Fact]
    public async Task Handle_WhenEventDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(new DeleteEventCommand { Id = EventsTestContext.MissingId }, CancellationToken.None));

        Assert.Contains($"Event with Id {EventsTestContext.MissingId}", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenEventAlreadySoftDeleted_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        await handler.Handle(new DeleteEventCommand { Id = seeded.Id }, CancellationToken.None);
        ctx.Db.ChangeTracker.Clear();

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(new DeleteEventCommand { Id = seeded.Id }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithExistingEvent_SoftDeletesInsteadOfRemovingTheRow()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var result = await handler.Handle(new DeleteEventCommand { Id = seeded.Id }, CancellationToken.None);

        Assert.Equal(Unit.Value, result);

        await using var readContext = ctx.NewContext();

        Assert.False(await readContext.Events.AnyAsync(x => x.Id == seeded.Id));

        var stored = await readContext.Events
            .IgnoreQueryFilters()
            .AsNoTracking()
            .SingleAsync(x => x.Id == seeded.Id);

        Assert.True(stored.IsDeleted);
    }

    [Fact]
    public async Task Handle_WithExistingEvent_SoftDeletesItsPerformerLineUp()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync(
            performers: new[]
            {
                (EventsTestContext.PerformerId, new TimeOnly(20, 0)),
                (EventsTestContext.OtherPerformerId, new TimeOnly(22, 0))
            });

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        await handler.Handle(new DeleteEventCommand { Id = seeded.Id }, CancellationToken.None);

        Assert.Empty(await ctx.GetPerformerEventsAsync(seeded.Id));

        await using var readContext = ctx.NewContext();
        var stored = await readContext.PerformerEvents
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.EventId == seeded.Id)
            .ToListAsync();

        Assert.Equal(2, stored.Count);
        Assert.All(stored, x => Assert.True(x.IsDeleted));
    }

    [Fact]
    public async Task Handle_WithExistingEvent_DeletesTheStoredImage()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync(image: "events/poster.png");
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        await handler.Handle(new DeleteEventCommand { Id = seeded.Id }, CancellationToken.None);

        Assert.Equal("events/poster.png", Assert.Single(ctx.ImageStorage.Deleted));
    }

    [Fact]
    public async Task Handle_WhenEventHasNoImage_DeletesNothingFromStorage()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        await handler.Handle(new DeleteEventCommand { Id = seeded.Id }, CancellationToken.None);

        Assert.Empty(ctx.ImageStorage.Deleted);
    }

    [Fact]
    public async Task Handle_WhenCallerIsAdmin_DeletesEventOfAnyOrganizer()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var foreignEvent = await ctx.AddEventAsync(organizerId: EventsTestContext.OtherOrganizerId);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        await handler.Handle(new DeleteEventCommand { Id = foreignEvent.Id }, CancellationToken.None);

        await using var readContext = ctx.NewContext();
        Assert.False(await readContext.Events.AnyAsync(x => x.Id == foreignEvent.Id));
    }
}
