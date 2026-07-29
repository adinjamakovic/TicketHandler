using Market.Application.Common;
using Market.Application.Modules.Events.Events.Queries.List;
using Market.Tests.Common;

namespace Market.Tests.EventTests.UnitTests;

public class ListEventsQueryHandlerTests
{
    private static readonly DateTime June = new(2026, 6, 10, 20, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime July = new(2026, 7, 20, 20, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime August = new(2026, 8, 30, 20, 0, 0, DateTimeKind.Utc);

    private static ListEventsQueryHandler CreateHandler(EventsTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser, ctx.ImageStorage);

    /// <summary>
    /// Three events: two for the primary organizer (Sarajevo / Concert and Mostar / Theatre)
    /// and one for the competing organizer.
    /// </summary>
    private static async Task SeedCatalogueAsync(EventsTestContext ctx)
    {
        await ctx.AddEventAsync(
            "Summer Rock Festival",
            scheduledDate: July,
            venueId: EventsTestContext.VenueId,
            eventTypeId: EventsTestContext.EventTypeId);

        await ctx.AddEventAsync(
            "Autumn Drama Nights",
            scheduledDate: August,
            venueId: EventsTestContext.OtherVenueId,
            eventTypeId: EventsTestContext.OtherEventTypeId);

        await ctx.AddEventAsync(
            "Mostar Jazz Evening",
            organizerId: EventsTestContext.OtherOrganizerId,
            scheduledDate: June,
            venueId: EventsTestContext.OtherVenueId,
            eventTypeId: EventsTestContext.EventTypeId);
    }

    [Fact]
    public async Task Handle_ForAnonymousCaller_ReturnsAllEventsOrderedByScheduledDate()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        await SeedCatalogueAsync(ctx);

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(new ListEventsQuery(), CancellationToken.None);

        Assert.Equal(3, result.Total);
        Assert.Equal(
            new[] { "Mostar Jazz Evening", "Summer Rock Festival", "Autumn Drama Nights" },
            result.Items.Select(x => x.Name));
    }

    [Fact]
    public async Task Handle_ProjectsOrganizerVenueAndImageDetails()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        await ctx.AddEventAsync("Summer Rock Festival", scheduledDate: July, image: "events/poster.png");

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(new ListEventsQuery(), CancellationToken.None);

        var item = Assert.Single(result.Items);

        Assert.Equal("Summer Rock Festival", item.Name);
        Assert.Equal("Mirza Delibasic Hall", item.VenueName);
        Assert.Equal("Sarajevo", item.VenueCity);
        Assert.Equal("Concert", item.EventType);
        Assert.Equal($"{FakeImageStorage.PublicPrefix}Events/events/poster.png", item.Image);

        Assert.Equal(EventsTestContext.OrganizerId, item.Organizer.Id);
        Assert.Equal("Sarajevo Events", item.Organizer.Name);
        Assert.Equal("Marsala Tita 1", item.Organizer.Address);
        Assert.Equal("Sarajevo", item.Organizer.City);
        Assert.Equal("organiser.one", item.Organizer.UserName);
    }

    [Theory]
    [InlineData("summer")]
    [InlineData("SUMMER")]
    [InlineData("  rock  ")]
    public async Task Handle_WithSearch_MatchesNameCaseInsensitivelyAndTrimmed(string search)
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        await SeedCatalogueAsync(ctx);

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(new ListEventsQuery { Search = search }, CancellationToken.None);

        Assert.Equal("Summer Rock Festival", Assert.Single(result.Items).Name);
    }

    [Fact]
    public async Task Handle_WithCityFilter_ReturnsOnlyEventsAtVenuesInThatCity()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        await SeedCatalogueAsync(ctx);

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(new ListEventsQuery { City = "mostar" }, CancellationToken.None);

        Assert.Equal(2, result.Total);
        Assert.All(result.Items, x => Assert.Equal("Mostar", x.VenueCity));
    }

    [Fact]
    public async Task Handle_WithEventTypeFilter_ReturnsOnlyMatchingType()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        await SeedCatalogueAsync(ctx);

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(new ListEventsQuery { EventType = "Theatre" }, CancellationToken.None);

        Assert.Equal("Autumn Drama Nights", Assert.Single(result.Items).Name);
    }

    [Fact]
    public async Task Handle_WithDateRange_ReturnsOnlyEventsInsideTheRangeInclusive()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        await SeedCatalogueAsync(ctx);

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(
                new ListEventsQuery { DateFrom = June, DateTo = July },
                CancellationToken.None);

        Assert.Equal(2, result.Total);
        Assert.Equal(
            new[] { "Mostar Jazz Evening", "Summer Rock Festival" },
            result.Items.Select(x => x.Name));
    }

    [Fact]
    public async Task Handle_WithOnlyDateFrom_DoesNotFilter()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        await SeedCatalogueAsync(ctx);

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(new ListEventsQuery { DateFrom = August }, CancellationToken.None);

        Assert.Equal(3, result.Total);
    }

    [Fact]
    public async Task Handle_ForOrganiser_ReturnsOnlyTheirOwnEvents()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        await SeedCatalogueAsync(ctx);

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OrganizerUserId))
            .Handle(new ListEventsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Total);
        Assert.All(result.Items, x => Assert.Equal(EventsTestContext.OrganizerId, x.Organizer.Id));
    }

    [Fact]
    public async Task Handle_ForOtherOrganiser_ReturnsOnlyTheirOwnEvents()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        await SeedCatalogueAsync(ctx);

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventsTestContext.OtherOrganizerUserId))
            .Handle(new ListEventsQuery(), CancellationToken.None);

        Assert.Equal("Mostar Jazz Evening", Assert.Single(result.Items).Name);
    }

    [Fact]
    public async Task Handle_WithPaging_ReturnsRequestedSliceAndMetadata()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        await SeedCatalogueAsync(ctx);

        var query = new ListEventsQuery { Paging = new PageRequest { Page = 2, PageSize = 2 } };

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(query, CancellationToken.None);

        Assert.Equal(3, result.Total);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(2, result.PageSize);
        Assert.True(result.IncludedTotal);
        Assert.Equal("Autumn Drama Nights", Assert.Single(result.Items).Name);
    }

    [Fact]
    public async Task Handle_WhenNothingMatches_ReturnsEmptyPage()
    {
        await using var ctx = await EventsTestContext.CreateAsync();
        await SeedCatalogueAsync(ctx);

        var result = await CreateHandler(ctx, FakeAppCurrentUser.Anonymous())
            .Handle(new ListEventsQuery { Search = "no such event" }, CancellationToken.None);

        Assert.Equal(0, result.Total);
        Assert.Empty(result.Items);
    }
}
