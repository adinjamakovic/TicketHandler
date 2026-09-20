using Market.Application.Modules.Events.Organizers.Queries.List;

namespace Market.Tests.OrganizerTests.UnitTests;

public class ListOrganizersQueryHandlerTests
{
    private static ListOrganizersQueryHandler CreateHandler(OrganizersTestContext ctx) =>
        new(ctx.Db, ctx.ImageStorage);

    /// <summary>Seeds one organizer on the clock's start day and a second one ten days later.</summary>
    private static async Task SeedTwoRegistrationDatesAsync(OrganizersTestContext ctx)
    {
        await ctx.AddOrganizerAsync(OrganizersTestContext.OrganizerUserId, "Sarajevo Events");

        ctx.Clock.Advance(TimeSpan.FromDays(10));

        await ctx.AddOrganizerAsync(
            OrganizersTestContext.OtherOrganizerUserId,
            "Mostar Events",
            OrganizersTestContext.MostarCityId);
    }

    [Fact]
    public async Task Handle_WithoutRegistrationFilter_ReturnsEveryOrganizer()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        await SeedTwoRegistrationDatesAsync(ctx);

        var result = await CreateHandler(ctx).Handle(new ListOrganizersQuery(), CancellationToken.None);

        Assert.Equal(2, result.Total);
    }

    [Fact]
    public async Task Handle_WithRegisteredFrom_KeepsOnlyOrganizersRegisteredOnOrAfterThatDay()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        await SeedTwoRegistrationDatesAsync(ctx);

        var result = await CreateHandler(ctx).Handle(
            new ListOrganizersQuery { RegisteredFrom = new DateTime(2026, 1, 20) },
            CancellationToken.None);

        Assert.Equal("Mostar Events", Assert.Single(result.Items).Name);
    }

    [Fact]
    public async Task Handle_WithRegisteredTo_KeepsOnlyOrganizersRegisteredOnOrBeforeThatDay()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        await SeedTwoRegistrationDatesAsync(ctx);

        var result = await CreateHandler(ctx).Handle(
            new ListOrganizersQuery { RegisteredTo = new DateTime(2026, 1, 20) },
            CancellationToken.None);

        Assert.Equal("Sarajevo Events", Assert.Single(result.Items).Name);
    }

    [Fact]
    public async Task Handle_WithRegistrationRange_IsInclusiveOnBothBounds()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        await SeedTwoRegistrationDatesAsync(ctx);

        var result = await CreateHandler(ctx).Handle(
            new ListOrganizersQuery
            {
                RegisteredFrom = new DateTime(2026, 1, 15),
                RegisteredTo = new DateTime(2026, 1, 25)
            },
            CancellationToken.None);

        Assert.Equal(2, result.Total);
    }

    [Fact]
    public async Task Handle_IgnoresTheTimeOfDayOnTheRegistrationBounds()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        await SeedTwoRegistrationDatesAsync(ctx);

        // The clock stamps the first organizer at 12:00, so an end bound earlier in the day
        // must still keep it — the filter compares calendar days, not instants.
        var result = await CreateHandler(ctx).Handle(
            new ListOrganizersQuery { RegisteredTo = new DateTime(2026, 1, 15, 6, 0, 0) },
            CancellationToken.None);

        Assert.Equal("Sarajevo Events", Assert.Single(result.Items).Name);
    }

    [Fact]
    public async Task Handle_CombinesTheRegistrationRangeWithTheOtherFilters()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        await SeedTwoRegistrationDatesAsync(ctx);

        var result = await CreateHandler(ctx).Handle(
            new ListOrganizersQuery
            {
                City = "Sarajevo",
                RegisteredFrom = new DateTime(2026, 1, 20)
            },
            CancellationToken.None);

        Assert.Equal(0, result.Total);
        Assert.Empty(result.Items);
    }
}
