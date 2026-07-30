using MediatR;
using Market.Application.Common.Exceptions;
using Market.Application.Modules.Events.EventsNews.Commands.Update;
using Market.Tests.Common;

namespace Market.Tests.EventNewsTests.UnitTests;

public class UpdateEventNewsCommandHandlerTests
{
    private static UpdateEventNewsCommandHandler CreateHandler(EventNewsTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser, ctx.ImageStorage);

    private static UpdateEventNewsCommand ValidCommand(int id) => new()
    {
        Id = id,
        Header = "Doors now open at 18:30",
        Body = "The schedule moved half an hour earlier."
    };

    [Fact]
    public async Task Handle_WhenNewsDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(ValidCommand(EventNewsTestContext.MissingId), CancellationToken.None));

        Assert.Equal("Event news does not exist", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCallerIsPlainUser_ThrowsBusinessRule()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User());

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(EventNewsTestContext.RockNightNewsId), CancellationToken.None));

        Assert.Equal("Only organisers can edit EventNews", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCallerIsAdmin_ThrowsBusinessRule()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(EventNewsTestContext.RockNightNewsId), CancellationToken.None));

        Assert.Equal("Only organisers can edit EventNews", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenNewsBelongsToAnotherOrganiser_ThrowsBusinessRule()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OtherOrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(EventNewsTestContext.RockNightNewsId), CancellationToken.None));

        Assert.Equal("Only the organiser who made the EventNews can edit them", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenOrganiserHasNoOrganizerRecord_ThrowsBusinessRule()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.MissingId));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(EventNewsTestContext.RockNightNewsId), CancellationToken.None));

        Assert.Equal("Only the organiser who made the EventNews can edit them", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenRejected_LeavesTheNewsUntouched()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OtherOrganizerUserId));

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(EventNewsTestContext.RockNightNewsId), CancellationToken.None));

        var stored = await ctx.GetEventNewsAsync(EventNewsTestContext.RockNightNewsId);

        Assert.Equal("Doors open at 19:00", stored!.Header);
        Assert.Equal(EventNewsTestContext.SeededNewsImage, stored.Image);
    }

    [Fact]
    public async Task Handle_WithValidCommand_UpdatesHeaderAndBody()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync("Original header", "Original body");
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        var result = await handler.Handle(ValidCommand(seeded.Id), CancellationToken.None);

        Assert.Equal(Unit.Value, result);

        var stored = await ctx.GetEventNewsAsync(seeded.Id);

        Assert.Equal("Doors now open at 18:30", stored!.Header);
        Assert.Equal("The schedule moved half an hour earlier.", stored.Body);
    }

    [Fact]
    public async Task Handle_WithValidCommand_KeepsOwnerAndEventUnchanged()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        await handler.Handle(ValidCommand(seeded.Id), CancellationToken.None);

        var stored = await ctx.GetEventNewsAsync(seeded.Id);

        Assert.Equal(EventNewsTestContext.OrganizerId, stored!.OrganizerId);
        Assert.Equal(EventNewsTestContext.RockNightEventId, stored.EventId);
        Assert.False(stored.IsDeleted);
    }

    [Fact]
    public async Task Handle_WithValidCommand_StampsModifiedAtFromTheClock()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        ctx.Clock.Advance(TimeSpan.FromHours(2));

        await handler.Handle(ValidCommand(seeded.Id), CancellationToken.None);

        var stored = await ctx.GetEventNewsAsync(seeded.Id);

        Assert.Equal(ctx.Clock.GetUtcNow().UtcDateTime, stored!.ModifiedAtUtc);
    }

    [Fact]
    public async Task Handle_WithoutNewImage_KeepsTheStoredImageAndTouchesNoStorage()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync(image: "event-news/original.png");
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        await handler.Handle(ValidCommand(seeded.Id), CancellationToken.None);

        Assert.Equal("event-news/original.png", (await ctx.GetEventNewsAsync(seeded.Id))!.Image);
        Assert.Empty(ctx.ImageStorage.Saved);
        Assert.Empty(ctx.ImageStorage.Deleted);
    }

    [Fact]
    public async Task Handle_WithNewImage_ReplacesTheStoredImageAndDeletesTheOldOne()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync(image: "event-news/original.png");
        ctx.ImageStorage.SavedPath = "event-news/replacement.png";

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));
        var command = ValidCommand(seeded.Id);
        command.Image = new FakeFormFile("replacement.png");

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal("event-news/replacement.png", (await ctx.GetEventNewsAsync(seeded.Id))!.Image);
        Assert.Equal("event-news/replacement.png", Assert.Single(ctx.ImageStorage.Saved));
        Assert.Equal("event-news/original.png", Assert.Single(ctx.ImageStorage.Deleted));
    }

    [Fact]
    public async Task Handle_WithFirstImage_SavesItWithoutDeletingAnything()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync();
        ctx.ImageStorage.SavedPath = "event-news/first.png";

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));
        var command = ValidCommand(seeded.Id);
        command.Image = new FakeFormFile("first.png");

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal("event-news/first.png", (await ctx.GetEventNewsAsync(seeded.Id))!.Image);
        Assert.Empty(ctx.ImageStorage.Deleted);
    }

    [Fact]
    public async Task Handle_WithEmptyImage_KeepsTheStoredImage()
    {
        await using var ctx = await EventNewsTestContext.CreateAsync();
        var seeded = await ctx.AddEventNewsAsync(image: "event-news/original.png");
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(EventNewsTestContext.OrganizerUserId));

        var command = ValidCommand(seeded.Id);
        command.Image = new FakeFormFile("replacement.png", length: 0);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal("event-news/original.png", (await ctx.GetEventNewsAsync(seeded.Id))!.Image);
        Assert.Empty(ctx.ImageStorage.Saved);
        Assert.Empty(ctx.ImageStorage.Deleted);
    }
}
