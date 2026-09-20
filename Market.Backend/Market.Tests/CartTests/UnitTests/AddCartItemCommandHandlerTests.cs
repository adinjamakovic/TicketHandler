using Market.Application.Common.Exceptions;
using Market.Application.Modules.Sales.Cart.Commands.AddItem;
using Market.Tests.Common;

namespace Market.Tests.CartTests.UnitTests;

public class AddCartItemCommandHandlerTests
{
    private static AddCartItemCommandHandler CreateHandler(
        CartTestContext ctx,
        FakeAppCurrentUser currentUser) => new(ctx.Db, currentUser);

    [Fact]
    public async Task Handle_OnASavedLine_PullsItBackIntoTheCartAndAddsTheQuantity()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(quantityInStock: 10);
        await ctx.AddCartItemAsync(ticket.Id, quantity: 2, isSavedForLater: true);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        await handler.Handle(
            new AddCartItemCommand { TicketId = ticket.Id, Quantity = 1 },
            CancellationToken.None);

        var stored = await ctx.GetCartItemAsync(ticket.Id);

        Assert.False(stored!.IsSavedForLater);
        Assert.Equal(3, stored.Quantity);
    }

    [Fact]
    public async Task Handle_WhenSavedQuantityAlreadyUsesUpStock_SaysWhereThoseTicketsAre()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(quantityInStock: 2);
        await ctx.AddCartItemAsync(ticket.Id, quantity: 2, isSavedForLater: true);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(
                new AddCartItemCommand { TicketId = ticket.Id, Quantity = 1 },
                CancellationToken.None));

        Assert.Equal("CART_OUT_OF_STOCK", ex.Code);
        Assert.Contains("saved for later", ex.Message);

        // The rejected add leaves the shelf exactly as it was.
        var stored = await ctx.GetCartItemAsync(ticket.Id);
        Assert.True(stored!.IsSavedForLater);
        Assert.Equal(2, stored.Quantity);
    }

    [Fact]
    public async Task Handle_OnANewTicket_AddsAnUnsavedLine()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        await handler.Handle(
            new AddCartItemCommand { TicketId = ticket.Id, Quantity = 2 },
            CancellationToken.None);

        var stored = await ctx.GetCartItemAsync(ticket.Id);

        Assert.False(stored!.IsSavedForLater);
        Assert.Equal(2, stored.Quantity);
    }
}
