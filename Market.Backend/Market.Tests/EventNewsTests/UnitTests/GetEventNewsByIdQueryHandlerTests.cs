using Market.Application.Abstractions;
using Market.Application.Common.Exceptions;
using Market.Application.Modules.Events.EventsNews.Commands.Delete;
using Market.Application.Modules.Events.EventsNews.Queries.GetById;
using Market.Tests.Common;

namespace Market.Tests.EventNewsTests.UnitTests;

public class GetEventNewsByIdQueryHandlerTests
{
    private static GetEventNewsByIdQueryHandler CreateHandler(
        EventNewsTestContext ctx,
        FakeAppCurrentUser? currentUser = null) =>
        new(ctx.Db, currentUser ?? FakeAppCurrentUser.Anonymous(), ctx.ImageStorage);

    [Fact]
    public async Task Handle_WhenNewsDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => CreateHandler(ctx).Handle(
                new GetEventNewsByIdQuery { Id = EventNewsTestContext.MissingId },
                CancellationToken.None));

        Assert.Equal("Event News not found", ex.Message);
    }

    [Fact]
    public async Task Handle_WithExistingNews_ProjectsRelatedNames()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var dto = await CreateHandler(ctx).Handle(
            new GetEventNewsByIdQuery { Id = EventNewsTestContext.RockNightNewsId },
            CancellationToken.None);

        Assert.Equal(EventNewsTestContext.RockNightNewsId, dto.Id);
        Assert.Equal("Rock Night Sarajevo", dto.Event);
        Assert.Equal("Sarajevo Events", dto.Organizer);
        Assert.Equal("Doors open at 19:00", dto.Header);
        Assert.Equal("Gates open one hour before the first act.", dto.Body);
    }

    [Fact]
    public async Task Handle_WithStoredImage_ReturnsPublicPath()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var dto = await CreateHandler(ctx).Handle(
            new GetEventNewsByIdQuery { Id = EventNewsTestContext.RockNightNewsId },
            CancellationToken.None);

        Assert.Equal(
            ctx.ImageStorage.ToPublicPath(ImageStorageCategory.EventNews, EventNewsTestContext.SeededNewsImage),
            dto.Image);
    }

    [Fact]
    public async Task Handle_WithoutStoredImage_ReturnsNullImage()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var dto = await CreateHandler(ctx).Handle(
            new GetEventNewsByIdQuery { Id = EventNewsTestContext.RockNightSecondNewsId },
            CancellationToken.None);

        Assert.Null(dto.Image);
    }

    [Fact]
    public async Task Handle_WithNullBody_ReturnsEmptyStringInsteadOfNull()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync(body: null);

        var dto = await CreateHandler(ctx).Handle(
            new GetEventNewsByIdQuery { Id = seeded.Id },
            CancellationToken.None);

        Assert.Equal(string.Empty, dto.Body);
    }

    [Theory]
    [InlineData("anonymous")]
    [InlineData("user")]
    [InlineData("other-organiser")]
    public async Task Handle_ReturnsTheSameNewsForEveryCaller(string callerKind)
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var caller = callerKind switch
        {
            "user" => FakeAppCurrentUser.User(),
            "other-organiser" => FakeAppCurrentUser.Organiser(EventNewsTestContext.OtherOrganizerUserId),
            _ => FakeAppCurrentUser.Anonymous()
        };

        var dto = await CreateHandler(ctx, caller).Handle(
            new GetEventNewsByIdQuery { Id = EventNewsTestContext.RockNightNewsId },
            CancellationToken.None);

        Assert.Equal("Doors open at 19:00", dto.Header);
    }

    [Fact]
    public async Task Handle_WhenNewsWasSoftDeleted_ThrowsNotFound()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync();

        var deleteHandler = new DeleteEventNewsCommandHandler(
            ctx.Db,
            FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId),
            ctx.ImageStorage);

        await deleteHandler.Handle(new DeleteEventNewsCommand { Id = seeded.Id }, CancellationToken.None);
        ctx.Db.ChangeTracker.Clear();

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => CreateHandler(ctx).Handle(
                new GetEventNewsByIdQuery { Id = seeded.Id },
                CancellationToken.None));
    }
}
