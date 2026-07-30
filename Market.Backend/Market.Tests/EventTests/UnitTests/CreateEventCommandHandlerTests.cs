using Market.Application.Common.Exceptions;
using Market.Application.Modules.Events.Events.Commands.Create;
using Market.Tests.Common;

namespace Market.Tests.EventTests.UnitTests;

public class CreateEventCommandHandlerTests
{
    private static CreateEventCommandHandler CreateHandler(EventsTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser, ctx.ImageStorage);

    private static CreateEventCommand ValidCommand(EventsTestContext ctx) => new()
    {
        Name = "Rock Night Sarajevo",
        Description = "Open air concert",
        ScheduledDate = ctx.FutureDate,
        VenueId = EventsTestContext.VenueId,
        EventTypeId = EventsTestContext.EventTypeId
    };

    [Fact]
    public async Task Handle_WhenCallerIsNotOrganiser_ThrowsBusinessRule()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User());

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(ctx), CancellationToken.None));

        Assert.Equal("Only an organiser can add Events", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenOrganiserHasNoOrganizerRecord_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.MissingId));

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(ValidCommand(ctx), CancellationToken.None));

        Assert.Equal("No organizer found", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenVenueDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx);
        command.VenueId = EventsTestContext.MissingId;

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Contains($"Venue with an Id of {EventsTestContext.MissingId}", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenEventTypeDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx);
        command.EventTypeId = EventsTestContext.MissingId;

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Event type not found", ex.Message);
    }

    [Fact]
    public async Task Handle_WithValidCommand_PersistsEventUnderCallersOrganizer()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));
        var command = ValidCommand(ctx);

        var id = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(0, id);

        await using var readContext = ctx.NewContext();
        var created = await readContext.Events.AsNoTracking().SingleAsync(x => x.Id == id);

        Assert.Equal(command.Name, created.Name);
        Assert.Equal(command.Description, created.Description);
        Assert.Equal(command.ScheduledDate, created.ScheduledDate);
        Assert.Equal(EventsTestContext.VenueId, created.VenueId);
        Assert.Equal(EventsTestContext.EventTypeId, created.EventTypeId);
        Assert.Equal(EventsTestContext.OrganizerId, created.OrganizerId);
        Assert.False(created.IsDeleted);
    }

    [Fact]
    public async Task Handle_WithoutImage_LeavesImageNullAndUploadsNothing()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var id = await handler.Handle(ValidCommand(ctx), CancellationToken.None);

        await using var readContext = ctx.NewContext();
        var created = await readContext.Events.AsNoTracking().SingleAsync(x => x.Id == id);

        Assert.Null(created.Image);
        Assert.Empty(ctx.ImageStorage.Saved);
    }

    [Fact]
    public async Task Handle_WithImage_StoresPathReturnedByImageStorage()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        ctx.ImageStorage.SavedPath = "events/rock-night.png";

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));
        var command = ValidCommand(ctx);
        command.Image = new FakeFormFile("rock-night.png");

        var id = await handler.Handle(command, CancellationToken.None);

        await using var readContext = ctx.NewContext();
        var created = await readContext.Events.AsNoTracking().SingleAsync(x => x.Id == id);

        Assert.Equal("events/rock-night.png", created.Image);
        Assert.Equal("events/rock-night.png", Assert.Single(ctx.ImageStorage.Saved));
    }

    [Fact]
    public async Task Handle_WithPerformers_CreatesPerformerEventRows()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx);
        command.Performers =
        [
            new CreateEventCommandPerformer { PerformerId = EventsTestContext.PerformerId, TimeStamp = new TimeOnly(20, 0) },
            new CreateEventCommandPerformer { PerformerId = EventsTestContext.OtherPerformerId, TimeStamp = new TimeOnly(22, 30) }
        ];

        var id = await handler.Handle(command, CancellationToken.None);

        var performerEvents = await ctx.GetPerformerEventsAsync(id);

        Assert.Equal(2, performerEvents.Count);
        Assert.Equal(
            new[] { EventsTestContext.PerformerId, EventsTestContext.OtherPerformerId },
            performerEvents.Select(x => x.PerformerId));
        Assert.Equal(
            new[] { new TimeOnly(20, 0), new TimeOnly(22, 30) },
            performerEvents.Select(x => x.TimeStamp));
    }

    [Fact]
    public async Task Handle_WhenPerformerDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx);
        command.Performers =
        [
            new CreateEventCommandPerformer { PerformerId = EventsTestContext.MissingId, TimeStamp = new TimeOnly(20, 0) }
        ];

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Performer does not exist", ex.Message);
    }
}
