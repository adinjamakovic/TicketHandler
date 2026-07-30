using Market.Application.Common.Exceptions;
using Market.Application.Modules.Events.EventsNews.Commands.Create;
using Market.Tests.Common;

namespace Market.Tests.EventNewsTests.UnitTests;

public class CreateEventNewsCommandHandlerTests
{
    private static CreateEventNewsCommandHandler CreateHandler(EventNewsTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser, ctx.ImageStorage);

    private static CreateEventNewsCommand ValidCommand() => new()
    {
        EventId = EventNewsTestContext.RockNightEventId,
        Header = "Doors open at 19:00",
        Body = "Gates open one hour before the first act.",
        Image = null
    };

    [Fact]
    public async Task Handle_WhenCallerIsPlainUser_ThrowsBusinessRule()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User());

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(), CancellationToken.None));

        Assert.Equal("Only an organiser can enter event news", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCallerIsAdmin_ThrowsBusinessRule()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenCallerIsPlainUser_PersistsNothing()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User());

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(), CancellationToken.None));

        Assert.Equal(2, (await ctx.GetEventNewsForEventAsync(EventNewsTestContext.RockNightEventId)).Count);
        Assert.Empty(ctx.ImageStorage.Saved);
    }

    /// <summary>
    /// BUG: the handler dereferences the organizer lookup without a null check, so an
    /// authenticated organiser whose <c>OrganizerEntity</c> row is missing gets a 500 instead
    /// of the <see cref="MarketNotFoundException"/> ("No organizer found") that
    /// <c>CreateEventCommandHandler</c> throws for the same situation.
    /// </summary>
    [Fact]
    public async Task Handle_WhenOrganiserHasNoOrganizerRecord_ThrowsNullReference()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.MissingId));

        await Assert.ThrowsAsync<NullReferenceException>(
            () => handler.Handle(ValidCommand(), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithValidCommand_PersistsNewsUnderCallersOrganizer()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));
        var command = ValidCommand();

        var id = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(0, id);

        var created = await ctx.GetEventNewsAsync(id);

        Assert.NotNull(created);
        Assert.Equal(EventNewsTestContext.OrganizerId, created!.OrganizerId);
        Assert.Equal(EventNewsTestContext.RockNightEventId, created.EventId);
        Assert.Equal(command.Header, created.Header);
        Assert.Equal(command.Body, created.Body);
        Assert.False(created.IsDeleted);
    }

    [Fact]
    public async Task Handle_WithValidCommand_StampsCreatedAtFromTheClock()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        var id = await handler.Handle(ValidCommand(), CancellationToken.None);

        var created = await ctx.GetEventNewsAsync(id);

        Assert.Equal(ctx.Clock.GetUtcNow().UtcDateTime, created!.CreatedAtUtc);
        Assert.Null(created.ModifiedAtUtc);
    }

    [Fact]
    public async Task Handle_WithNullBody_StoresEmptyStringInsteadOfNull()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        var command = ValidCommand();
        command.Body = null;

        var id = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(string.Empty, (await ctx.GetEventNewsAsync(id))!.Body);
    }

    [Fact]
    public async Task Handle_WithImage_StoresPathReturnedByImageStorage()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        ctx.ImageStorage.SavedPath = "event-news/doors-open.png";

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));
        var command = ValidCommand();
        command.Image = new FakeFormFile("doors-open.png");

        var id = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("event-news/doors-open.png", (await ctx.GetEventNewsAsync(id))!.Image);
        Assert.Equal("event-news/doors-open.png", Assert.Single(ctx.ImageStorage.Saved));
    }

    [Fact]
    public async Task Handle_WithEmptyImage_UploadsNothing()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        var command = ValidCommand();
        command.Image = new FakeFormFile("doors-open.png", length: 0);

        var id = await handler.Handle(command, CancellationToken.None);

        Assert.Null((await ctx.GetEventNewsAsync(id))!.Image);
        Assert.Empty(ctx.ImageStorage.Saved);
    }

    [Fact]
    public async Task Handle_AppendsToTheNewsAlreadyOnTheEvent()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        var id = await handler.Handle(ValidCommand(), CancellationToken.None);

        var newsForEvent = await ctx.GetEventNewsForEventAsync(EventNewsTestContext.RockNightEventId);

        Assert.Equal(3, newsForEvent.Count);
        Assert.Contains(newsForEvent, x => x.Id == id);
    }
}
