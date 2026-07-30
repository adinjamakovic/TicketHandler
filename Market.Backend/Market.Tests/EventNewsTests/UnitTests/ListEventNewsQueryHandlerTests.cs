using Market.Application.Common;
using Market.Application.Modules.Events.EventsNews.Commands.Delete;
using Market.Application.Modules.Events.EventsNews.Queries.List;
using Market.Tests.Common;

namespace Market.Tests.EventNewsTests.UnitTests;
public class ListEventNewsQueryHandlerTests
{
    private static ListEventNewsQueryHandler CreateHandler(EventNewsTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser, ctx.ImageStorage);

    [Fact]
    public async Task Handle_ForAnonymousCaller_ReturnsEveryPost()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(new ListEventNewsQuery(), CancellationToken.None);

        Assert.Equal(3, result.Total);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task Handle_WithEventFilter_ReturnsOnlyThatEventsPosts()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(
                new ListEventNewsQuery { EventId = EventNewsTestContext.RockNightEventId },
                CancellationToken.None);

        Assert.Equal(2, result.Total);
        Assert.All(result.Items, x => Assert.Equal("Rock Night Sarajevo", x.Event));
    }

    [Fact]
    public async Task Handle_WithOrganizerFilter_ReturnsOnlyThatOrganizersPosts()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var result = await CreateHandler(ctx, FakeAppCurrentUser.User())
            .Handle(
                new ListEventNewsQuery { OrganizerId = EventNewsTestContext.OtherOrganizerId },
                CancellationToken.None);

        Assert.Equal("Camping site is open", Assert.Single(result.Items).Header);
    }

    [Fact]
    public async Task Handle_WithOrganizerAndEventFilter_AppliesBoth()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(
                new ListEventNewsQuery
                {
                    OrganizerId = EventNewsTestContext.OrganizerId,
                    EventId = EventNewsTestContext.SummerFestivalEventId
                },
                CancellationToken.None);

        Assert.Equal(0, result.Total);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Handle_ForOrganiser_ReturnsOnlyTheirOwnPosts()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId))
            .Handle(new ListEventNewsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Total);
        Assert.All(result.Items, x => Assert.Equal("Sarajevo Events", x.Organizer));
    }

    [Fact]
    public async Task Handle_ForOtherOrganiser_ReturnsOnlyTheirOwnPosts()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OtherOrganizerUserId))
            .Handle(new ListEventNewsQuery(), CancellationToken.None);

        Assert.Equal("Camping site is open", Assert.Single(result.Items).Header);
    }
    [Fact]
    public async Task Handle_ForOrganiser_OverwritesASuppliedOrganizerFilterWithTheirOwn()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var query = new ListEventNewsQuery { OrganizerId = EventNewsTestContext.OtherOrganizerId };

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId))
            .Handle(query, CancellationToken.None);

        Assert.Equal(EventNewsTestContext.OrganizerId, query.OrganizerId);
        Assert.Equal(2, result.Total);
        Assert.All(result.Items, x => Assert.Equal("Sarajevo Events", x.Organizer));
    }

    [Fact]
    public async Task Handle_ForOrganiser_StillHonoursTheEventFilterWithinTheirOwnPosts()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId))
            .Handle(
                new ListEventNewsQuery { EventId = EventNewsTestContext.SummerFestivalEventId },
                CancellationToken.None);

        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task Handle_ForAdmin_ReturnsEveryPost()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Admin())
            .Handle(new ListEventNewsQuery(), CancellationToken.None);

        Assert.Equal(3, result.Total);
    }

    [Fact]
    public async Task Handle_ProjectsEventOrganizerAndImageDetails()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(new ListEventNewsQuery { EventId = EventNewsTestContext.SummerFestivalEventId }, CancellationToken.None);

        var item = Assert.Single(result.Items);

        Assert.Equal(EventNewsTestContext.SummerFestivalNewsId, item.Id);
        Assert.Equal("Mostar Summer Festival", item.Event);
        Assert.Equal("Mostar Events", item.Organizer);
        Assert.Equal("Camping site is open", item.Header);
        Assert.Equal("The camping site opens the day before the festival.", item.Body);
        Assert.Null(item.Image);
    }

    [Fact]
    public async Task Handle_WithStoredImage_ReturnsPublicPath()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(new ListEventNewsQuery { EventId = EventNewsTestContext.RockNightEventId }, CancellationToken.None);

        var withImage = Assert.Single(result.Items, x => x.Id == EventNewsTestContext.RockNightNewsId);

        Assert.Equal(
            $"{FakeImageStorage.PublicPrefix}EventNews/{EventNewsTestContext.SeededNewsImage}",
            withImage.Image);
    }

    [Fact]
    public async Task Handle_OrdersOldestPostFirst()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        ctx.Clock.Advance(TimeSpan.FromHours(1));
        await ctx.AddEventNewsAsync(
            "Second announcement",
            organizerId: EventNewsTestContext.OtherOrganizerId,
            eventId: EventNewsTestContext.SummerFestivalEventId);

        ctx.Clock.Advance(TimeSpan.FromHours(1));
        await ctx.AddEventNewsAsync(
            "Third announcement",
            organizerId: EventNewsTestContext.OtherOrganizerId,
            eventId: EventNewsTestContext.SummerFestivalEventId);

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(new ListEventNewsQuery { EventId = EventNewsTestContext.SummerFestivalEventId }, CancellationToken.None);

        Assert.Equal(
            new[] { "Camping site is open", "Second announcement", "Third announcement" },
            result.Items.Select(x => x.Header));
    }

    [Fact]
    public async Task Handle_WithPaging_ReturnsRequestedSliceAndMetadata()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();

        ctx.Clock.Advance(TimeSpan.FromHours(1));
        await ctx.AddEventNewsAsync(
            "Second announcement",
            organizerId: EventNewsTestContext.OtherOrganizerId,
            eventId: EventNewsTestContext.SummerFestivalEventId);

        ctx.Clock.Advance(TimeSpan.FromHours(1));
        await ctx.AddEventNewsAsync(
            "Third announcement",
            organizerId: EventNewsTestContext.OtherOrganizerId,
            eventId: EventNewsTestContext.SummerFestivalEventId);

        var query = new ListEventNewsQuery
        {
            EventId = EventNewsTestContext.SummerFestivalEventId,
            Paging = new PageRequest { Page = 2, PageSize = 2 }
        };

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(query, CancellationToken.None);

        Assert.Equal(3, result.Total);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(2, result.PageSize);
        Assert.True(result.IncludedTotal);
        Assert.Equal("Third announcement", Assert.Single(result.Items).Header);
    }
}
