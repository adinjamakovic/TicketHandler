using Market.Application.Common.Exceptions;
using Market.Application.Modules.Sales.Tickets.Commands.Delete;
using Market.Tests.Common;

namespace Market.Tests.TicketTests.UnitTests;

public class DeleteTicketsCommandHandlerTests
{
    private static DeleteTicketsCommandHandler CreateHandler(TicketsTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser);

    [Fact]
    public async Task Handle_WhenCallerIsPlainUser_ThrowsBusinessRule()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User());

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(new DeleteTicketsCommand { Id = ticket.Id }, CancellationToken.None));

        Assert.Equal("Only an organiser can delete tickets", ex.Message);
        Assert.NotNull(await ctx.GetTicketAsync(ticket.Id));
    }

    [Fact]
    public async Task Handle_WhenTicketBelongsToAnotherOrganizer_ThrowsBusinessRule()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var foreignTicket = await ctx.AddTicketAsync(eventId: TicketsTestContext.SummerFestivalEventId);

        // Organizer A aims at a ticket on organizer B's festival.
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(TicketsTestContext.OrganizerUserId));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(new DeleteTicketsCommand { Id = foreignTicket.Id }, CancellationToken.None));

        Assert.Equal($"Ticket with Id {foreignTicket.Id} belongs to another organizer", ex.Message);
        Assert.NotNull(await ctx.GetTicketAsync(foreignTicket.Id));
    }

    [Fact]
    public async Task Handle_WhenCallerIsAdmin_ThrowsBusinessRule()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(new DeleteTicketsCommand { Id = ticket.Id }, CancellationToken.None));

        Assert.NotNull(await ctx.GetTicketAsync(ticket.Id));
    }

    [Fact]
    public async Task Handle_WhenTicketDoesNotExist_ThrowsNotFound()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(TicketsTestContext.OrganizerUserId));

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(
                new DeleteTicketsCommand { Id = TicketsTestContext.MissingId },
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_OnOwnTicket_SoftDeletesIt()
    {
        await using var ctx = await TicketsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(TicketsTestContext.OrganizerUserId));

        await handler.Handle(new DeleteTicketsCommand { Id = ticket.Id }, CancellationToken.None);

        Assert.Null(await ctx.GetTicketAsync(ticket.Id));

        await using var readContext = ctx.NewContext();
        var stored = await readContext.Tickets
            .IgnoreQueryFilters()
            .AsNoTracking()
            .SingleAsync(x => x.Id == ticket.Id);

        Assert.True(stored.IsDeleted);
    }
}
