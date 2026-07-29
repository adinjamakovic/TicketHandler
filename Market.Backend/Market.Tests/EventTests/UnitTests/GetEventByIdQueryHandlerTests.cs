using Market.Application.Abstractions;
using Market.Application.Common.Exceptions;
using Market.Application.Modules.Events.Events.Commands.Delete;
using Market.Application.Modules.Events.Events.Queries.GetById;
using Market.Tests.Common;

namespace Market.Tests.EventTests.UnitTests;

public class GetEventByIdQueryHandlerTests
{
    private static GetEventByIdQueryHandler CreateHandler(EventsTestContext ctx) =>
        new(ctx.Db, ctx.ImageStorage);

    [Fact]
    public async Task Handle_WhenEventDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var handler = CreateHandler(ctx);

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(new GetEventByIdQuery { Id = EventsTestContext.MissingId }, CancellationToken.None));

        Assert.Contains($"Event with Id {EventsTestContext.MissingId}", ex.Message);
    }

    [Fact]
    public async Task Handle_WithExistingEvent_ProjectsRelatedNames()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var scheduledDate = new DateTime(2026, 9, 1, 20, 0, 0, DateTimeKind.Utc);
        var seeded = await ctx.AddEventAsync("Rock Night Sarajevo", scheduledDate: scheduledDate);

        var dto = await CreateHandler(ctx).Handle(new GetEventByIdQuery { Id = seeded.Id }, CancellationToken.None);

        Assert.Equal(seeded.Id, dto.Id);
        Assert.Equal("Rock Night Sarajevo", dto.Name);
        Assert.Equal(scheduledDate, dto.ScheduledDate);
        Assert.Equal(EventsTestContext.VenueId, dto.VenueId);
        Assert.Equal("Mirza Delibasic Hall", dto.VenueName);
        Assert.Equal("Skenderija", dto.LocationName);
        Assert.Equal("Terezije bb", dto.LocationAddress);
        Assert.Equal("Sarajevo", dto.VenueCity);
        Assert.Equal(EventsTestContext.EventTypeId, dto.EventTypeId);
        Assert.Equal("Concert", dto.EventTypeName);
        Assert.Equal("Sarajevo Events", dto.OrganizerName);
    }

    [Fact]
    public async Task Handle_WithStoredImage_ReturnsPublicPath()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync(image: "events/poster.png");

        var dto = await CreateHandler(ctx).Handle(new GetEventByIdQuery { Id = seeded.Id }, CancellationToken.None);

        Assert.Equal(
            ctx.ImageStorage.ToPublicPath(ImageStorageCategory.Events, "events/poster.png"),
            dto.Image);
    }

    [Fact]
    public async Task Handle_WithoutStoredImage_ReturnsNullImage()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();

        var dto = await CreateHandler(ctx).Handle(new GetEventByIdQuery { Id = seeded.Id }, CancellationToken.None);

        Assert.Null(dto.Image);
    }

    [Fact]
    public async Task Handle_WithPerformers_ReturnsLineUpRowIdsSoEditsCanUpdateInPlace()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync(
            performers: new[]
            {
                (EventsTestContext.PerformerId, new TimeOnly(20, 0)),
                (EventsTestContext.OtherPerformerId, new TimeOnly(22, 30))
            });

        var expected = await ctx.GetPerformerEventsAsync(seeded.Id);

        var dto = await CreateHandler(ctx).Handle(new GetEventByIdQuery { Id = seeded.Id }, CancellationToken.None);

        Assert.Equal(2, dto.Performers.Count);
        Assert.Equal(
            expected.Select(x => x.Id),
            dto.Performers.OrderBy(x => x.Id).Select(x => x.Id));
        Assert.Equal(
            new[] { EventsTestContext.PerformerId, EventsTestContext.OtherPerformerId },
            dto.Performers.OrderBy(x => x.Id).Select(x => x.PerformerId));
        Assert.Equal(
            new[] { new TimeOnly(20, 0), new TimeOnly(22, 30) },
            dto.Performers.OrderBy(x => x.Id).Select(x => x.TimeStamp));
    }

    [Fact]
    public async Task Handle_WhenEventWasSoftDeleted_ThrowsNotFound()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        var seeded = await ctx.AddEventAsync();

        var deleteHandler = new DeleteEventCommandHandler(
            ctx.Db,
            FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId),
            ctx.ImageStorage);

        await deleteHandler.Handle(new DeleteEventCommand { Id = seeded.Id }, CancellationToken.None);
        ctx.Db.ChangeTracker.Clear();

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => CreateHandler(ctx).Handle(new GetEventByIdQuery { Id = seeded.Id }, CancellationToken.None));
    }
}
