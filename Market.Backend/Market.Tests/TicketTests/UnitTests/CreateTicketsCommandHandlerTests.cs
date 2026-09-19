using Market.Application.Common.Exceptions;
using Market.Application.Modules.Sales.Tickets.Commands.Create;
using Market.Tests.Common;

namespace Market.Tests.TicketTests.UnitTests;

public class CreateTicketsCommandHandlerTests
{
    private static CreateTicketsCommandHandler CreateHandler(TicketsTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser);

    private static CreateTicketsCommand ValidCommand(int eventId = TicketsTestContext.RockNightEventId) => new()
    {
        EventId = eventId,
        TicketTypeId = TicketsTestContext.RegularTicketTypeId,
        QuantityInStock = 250,
        UnitPrice = 45,
        Benefits = "Standing area entry"
    };

    [Fact]
    public async Task Handle_WhenCallerIsPlainUser_ThrowsBusinessRule()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User());

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(), CancellationToken.None));

        Assert.Equal("Only an organiser can create tickets", ex.Message);
        Assert.Equal(0, await ctx.CountTicketsAsync());
    }

    [Fact]
    public async Task Handle_WhenOrganiserHasNoOrganizerRecord_ThrowsNotFound()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(TicketsTestContext.MissingId));

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(ValidCommand(), CancellationToken.None));

        Assert.Equal("No organizer found", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenEventDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(TicketsTestContext.OrganizerUserId));

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(ValidCommand(TicketsTestContext.MissingId), CancellationToken.None));

        Assert.Equal(0, await ctx.CountTicketsAsync());
    }

    [Fact]
    public async Task Handle_WhenEventBelongsToAnotherOrganizer_ThrowsBusinessRule()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();

        // Organizer A tries to put tickets on organizer B's festival.
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(TicketsTestContext.OrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(TicketsTestContext.SummerFestivalEventId), CancellationToken.None));

        Assert.Equal(
            $"Event with Id {TicketsTestContext.SummerFestivalEventId} belongs to another organizer",
            ex.Message);
        Assert.Equal(0, await ctx.CountTicketsAsync());
    }

    [Fact]
    public async Task Handle_WhenCallerIsAdmin_ThrowsBusinessRule()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(), CancellationToken.None));

        Assert.Equal(0, await ctx.CountTicketsAsync());
    }

    [Fact]
    public async Task Handle_OnOwnEvent_PersistsTheTicket()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(TicketsTestContext.OrganizerUserId));

        var id = await handler.Handle(ValidCommand(), CancellationToken.None);

        var stored = await ctx.GetTicketAsync(id);

        Assert.NotNull(stored);
        Assert.Equal(TicketsTestContext.RockNightEventId, stored!.EventId);
        Assert.Equal(250, stored.QuantityInStock);
        Assert.Equal(45, stored.UnitPrice);
    }
}
