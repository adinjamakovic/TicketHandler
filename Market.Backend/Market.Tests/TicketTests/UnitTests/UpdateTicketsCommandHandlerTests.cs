using Market.Application.Common.Exceptions;
using Market.Application.Modules.Sales.Tickets.Commands.Update;
using Market.Tests.Common;

namespace Market.Tests.TicketTests.UnitTests;

public class UpdateTicketsCommandHandlerTests
{
    private static UpdateTicketsCommandHandler CreateHandler(TicketsTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser);

    private static UpdateTicketsCommand ValidCommand(
        int id,
        int eventId = TicketsTestContext.RockNightEventId) => new()
    {
        Id = id,
        EventId = eventId,
        TicketTypeId = TicketsTestContext.VipTicketTypeId,
        QuantityInStock = 10,
        UnitPrice = 150,
        Benefits = "Front row"
    };

    [Fact]
    public async Task Handle_WhenCallerIsPlainUser_ThrowsBusinessRule()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User());

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(ticket.Id), CancellationToken.None));

        Assert.Equal("Only an organiser can edit tickets", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenTicketBelongsToAnotherOrganizer_ThrowsBusinessRule()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();

        // Ticket sits on organizer B's festival...
        var foreignTicket = await ctx.AddTicketAsync(eventId: TicketsTestContext.SummerFestivalEventId);

        // ...and organizer A tries to repoint it at their own event.
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(TicketsTestContext.OrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(foreignTicket.Id), CancellationToken.None));

        Assert.Equal($"Ticket with Id {foreignTicket.Id} belongs to another organizer", ex.Message);

        var stored = await ctx.GetTicketAsync(foreignTicket.Id);
        Assert.Equal(TicketsTestContext.SummerFestivalEventId, stored!.EventId);
        Assert.Equal(50, stored.UnitPrice);
    }

    [Fact]
    public async Task Handle_WhenTargetEventBelongsToAnotherOrganizer_ThrowsBusinessRule()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var ownTicket = await ctx.AddTicketAsync();

        // Organizer A owns the ticket but tries to push it into organizer B's festival.
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(TicketsTestContext.OrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(
                ValidCommand(ownTicket.Id, TicketsTestContext.SummerFestivalEventId),
                CancellationToken.None));

        Assert.Equal(
            $"Event with Id {TicketsTestContext.SummerFestivalEventId} belongs to another organizer",
            ex.Message);

        var stored = await ctx.GetTicketAsync(ownTicket.Id);
        Assert.Equal(TicketsTestContext.RockNightEventId, stored!.EventId);
    }

    [Fact]
    public async Task Handle_WhenCallerIsAdmin_ThrowsBusinessRule()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(ticket.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenTicketDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(TicketsTestContext.OrganizerUserId));

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(ValidCommand(TicketsTestContext.MissingId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_OnOwnTicket_PersistsTheChanges()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(TicketsTestContext.OrganizerUserId));

        await handler.Handle(ValidCommand(ticket.Id), CancellationToken.None);

        var stored = await ctx.GetTicketAsync(ticket.Id);

        Assert.Equal(TicketsTestContext.VipTicketTypeId, stored!.TicketTypeId);
        Assert.Equal(10, stored.QuantityInStock);
        Assert.Equal(150, stored.UnitPrice);
        Assert.Equal("Front row", stored.Benefits);
    }
}
