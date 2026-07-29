using FluentValidation;
using Market.Application.Common.Exceptions;
using Market.Application.Modules.Events.Events.Commands.Update;
using Market.Tests.Common;

namespace Market.Tests.EventTests.UnitTests;

public class UpdateEventCommandHandlerTests
{
    private static UpdateEventCommandHandler CreateHandler(EventsTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser, ctx.ImageStorage);

    private static UpdateEventCommand ValidCommand(EventsTestContext ctx, int eventId) => new()
    {
        Id = eventId,
        Name = "Rock Night Sarajevo (updated)",
        Description = "Moved indoors",
        ScheduledDate = ctx.FutureDate,
        VenueId = EventsTestContext.VenueId,
        EventTypeId = EventsTestContext.EventTypeId
    };

    [Fact]
    public async Task Handle_WhenCallerIsNotOrganiser_ThrowsBusinessRule()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(ctx, seeded.Id), CancellationToken.None));

        Assert.Equal("Only organisers can update event details", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenVenueDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx, seeded.Id);
        command.VenueId = EventsTestContext.MissingId;

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Venue does not exist", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenEventTypeDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx, seeded.Id);
        command.EventTypeId = EventsTestContext.MissingId;

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Event type does not exist", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenEventDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(ValidCommand(ctx, EventsTestContext.MissingId), CancellationToken.None));

        Assert.Equal("This event does not exist", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenEventBelongsToAnotherOrganizer_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var foreignEvent = await ctx.AddEventAsync(organizerId: EventsTestContext.OtherOrganizerId);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(ValidCommand(ctx, foreignEvent.Id), CancellationToken.None));

        Assert.Equal("This event does not exist", ex.Message);
    }

    [Fact]
    public async Task Handle_WithValidCommand_UpdatesEditableFields()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx, seeded.Id);
        command.VenueId = EventsTestContext.OtherVenueId;
        command.EventTypeId = EventsTestContext.OtherEventTypeId;

        var returnedId = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(seeded.Id, returnedId);

        await using var readContext = ctx.NewContext();
        var updated = await readContext.Events.AsNoTracking().SingleAsync(x => x.Id == seeded.Id);

        Assert.Equal(command.Name, updated.Name);
        Assert.Equal(command.Description, updated.Description);
        Assert.Equal(command.ScheduledDate, updated.ScheduledDate);
        Assert.Equal(EventsTestContext.OtherVenueId, updated.VenueId);
        Assert.Equal(EventsTestContext.OtherEventTypeId, updated.EventTypeId);
        Assert.Equal(EventsTestContext.OrganizerId, updated.OrganizerId);
    }

    [Fact]
    public async Task Handle_WithoutNewImage_KeepsExistingImage()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync(image: "events/original.png");
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        await handler.Handle(ValidCommand(ctx, seeded.Id), CancellationToken.None);

        await using var readContext = ctx.NewContext();
        var updated = await readContext.Events.AsNoTracking().SingleAsync(x => x.Id == seeded.Id);

        Assert.Equal("events/original.png", updated.Image);
        Assert.Empty(ctx.ImageStorage.Saved);
        Assert.Empty(ctx.ImageStorage.Deleted);
    }

    [Fact]
    public async Task Handle_WithNewImage_ReplacesStoredImageAndDeletesOldOne()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync(image: "events/original.png");
        ctx.ImageStorage.SavedPath = "events/replacement.png";

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));
        var command = ValidCommand(ctx, seeded.Id);
        command.Image = new FakeFormFile("replacement.png");

        await handler.Handle(command, CancellationToken.None);

        await using var readContext = ctx.NewContext();
        var updated = await readContext.Events.AsNoTracking().SingleAsync(x => x.Id == seeded.Id);

        Assert.Equal("events/replacement.png", updated.Image);
        Assert.Equal("events/replacement.png", Assert.Single(ctx.ImageStorage.Saved));
        Assert.Equal("events/original.png", Assert.Single(ctx.ImageStorage.Deleted));
    }

    [Fact]
    public async Task Handle_WithNewPerformerEntry_AddsPerformerEventRow()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx, seeded.Id);
        command.Performers =
        [
            new UpdateEventCommandPerformers
            {
                Id = 0,
                PerformerId = EventsTestContext.PerformerId,
                TimeStamp = new TimeOnly(21, 0)
            }
        ];

        await handler.Handle(command, CancellationToken.None);

        var performerEvent = Assert.Single(await ctx.GetPerformerEventsAsync(seeded.Id));
        Assert.Equal(EventsTestContext.PerformerId, performerEvent.PerformerId);
        Assert.Equal(new TimeOnly(21, 0), performerEvent.TimeStamp);
    }

    [Fact]
    public async Task Handle_WithExistingPerformerEntry_UpdatesTimeStampInPlace()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync(
            performers: new[] { (EventsTestContext.PerformerId, new TimeOnly(20, 0)) });

        var existing = Assert.Single(await ctx.GetPerformerEventsAsync(seeded.Id));
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx, seeded.Id);
        command.Performers =
        [
            new UpdateEventCommandPerformers
            {
                Id = existing.Id,
                PerformerId = EventsTestContext.PerformerId,
                TimeStamp = new TimeOnly(23, 15)
            }
        ];

        await handler.Handle(command, CancellationToken.None);

        var performerEvent = Assert.Single(await ctx.GetPerformerEventsAsync(seeded.Id));
        Assert.Equal(existing.Id, performerEvent.Id);
        Assert.Equal(new TimeOnly(23, 15), performerEvent.TimeStamp);
    }

    [Fact]
    public async Task Handle_WhenPerformerOmittedFromCommand_SoftDeletesTheLineUpRow()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync(
            performers: new[]
            {
                (EventsTestContext.PerformerId, new TimeOnly(20, 0)),
                (EventsTestContext.OtherPerformerId, new TimeOnly(22, 0))
            });

        var kept = (await ctx.GetPerformerEventsAsync(seeded.Id))
            .Single(x => x.PerformerId == EventsTestContext.PerformerId);

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx, seeded.Id);
        command.Performers =
        [
            new UpdateEventCommandPerformers
            {
                Id = kept.Id,
                PerformerId = EventsTestContext.PerformerId,
                TimeStamp = new TimeOnly(20, 0)
            }
        ];

        await handler.Handle(command, CancellationToken.None);

        var remaining = Assert.Single(await ctx.GetPerformerEventsAsync(seeded.Id));
        Assert.Equal(EventsTestContext.PerformerId, remaining.PerformerId);

        await using var readContext = ctx.NewContext();
        var removed = await readContext.PerformerEvents
            .IgnoreQueryFilters()
            .AsNoTracking()
            .SingleAsync(x => x.EventId == seeded.Id && x.PerformerId == EventsTestContext.OtherPerformerId);

        Assert.True(removed.IsDeleted);
    }

    [Fact]
    public async Task Handle_WhenPerformerDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx, seeded.Id);
        command.Performers =
        [
            new UpdateEventCommandPerformers
            {
                Id = 0,
                PerformerId = EventsTestContext.MissingId,
                TimeStamp = new TimeOnly(21, 0)
            }
        ];

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Performer does not exist", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenPerformerEntryBelongsToAnotherEvent_ThrowsValidation()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var target = await ctx.AddEventAsync("Target Event");
        var otherEvent = await ctx.AddEventAsync(
            "Other Event",
            performers: new[] { (EventsTestContext.PerformerId, new TimeOnly(20, 0)) });

        var foreignEntry = Assert.Single(await ctx.GetPerformerEventsAsync(otherEvent.Id));
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId));

        var command = ValidCommand(ctx, target.Id);
        command.Performers =
        [
            new UpdateEventCommandPerformers
            {
                Id = foreignEntry.Id,
                PerformerId = EventsTestContext.PerformerId,
                TimeStamp = new TimeOnly(21, 0)
            }
        ];

        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Contains("Performer was not found as part of this event", ex.Message);
    }
}
