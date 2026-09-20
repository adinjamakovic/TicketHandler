using Market.Application.Common.Exceptions;
using Market.Application.Modules.Sales.Cart.Commands.MoveToCart;
using Market.Tests.Common;

namespace Market.Tests.CartTests.UnitTests;

public class MoveCartItemToCartCommandHandlerTests
{
    private static MoveCartItemToCartCommandHandler CreateHandler(
        CartTestContext ctx,
        FakeAppCurrentUser currentUser) => new(ctx.Db, currentUser);

    [Fact]
    public async Task Handle_OnSavedLine_PutsItBackInTheCart()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(quantityInStock: 10);
        await ctx.AddCartItemAsync(ticket.Id, quantity: 4, isSavedForLater: true);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        await handler.Handle(new MoveCartItemToCartCommand { TicketId = ticket.Id }, CancellationToken.None);

        var stored = await ctx.GetCartItemAsync(ticket.Id);

        Assert.False(stored!.IsSavedForLater);
        Assert.Equal(4, stored.Quantity);
    }

    [Fact]
    public async Task Handle_WhenStockDroppedBelowTheSavedQuantity_ThrowsBusinessRuleAndLeavesItSaved()
    {
        await using var ctx = await CartTestContext.CreateAsync();

        // Saved with 5, but only 2 are left by the time the buyer comes back.
        var ticket = await ctx.AddTicketAsync(quantityInStock: 2);
        await ctx.AddCartItemAsync(ticket.Id, quantity: 5, isSavedForLater: true);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(
                new MoveCartItemToCartCommand { TicketId = ticket.Id },
                CancellationToken.None));

        Assert.Equal("CART_OUT_OF_STOCK", ex.Code);

        var stored = await ctx.GetCartItemAsync(ticket.Id);
        Assert.True(stored!.IsSavedForLater);
    }

    [Fact]
    public async Task Handle_WhenTheSavedQuantityExactlyMatchesStock_Succeeds()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(quantityInStock: 3);
        await ctx.AddCartItemAsync(ticket.Id, quantity: 3, isSavedForLater: true);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        await handler.Handle(new MoveCartItemToCartCommand { TicketId = ticket.Id }, CancellationToken.None);

        var stored = await ctx.GetCartItemAsync(ticket.Id);
        Assert.False(stored!.IsSavedForLater);
    }

    [Fact]
    public async Task Handle_WhenLineIsAlreadyInTheCart_IsANoOp()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        await ctx.AddCartItemAsync(ticket.Id, quantity: 2);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        await handler.Handle(new MoveCartItemToCartCommand { TicketId = ticket.Id }, CancellationToken.None);

        var stored = await ctx.GetCartItemAsync(ticket.Id);

        Assert.False(stored!.IsSavedForLater);
        Assert.Equal(2, stored.Quantity);
    }

    [Fact]
    public async Task Handle_WhenNothingIsSavedForThatTicket_ThrowsNotFound()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(
                new MoveCartItemToCartCommand { TicketId = CartTestContext.MissingId },
                CancellationToken.None));

        Assert.Equal($"Ticket with Id {CartTestContext.MissingId} is not saved for later", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenTheSavedLineWasRemoved_ThrowsNotFound()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        var cartItem = await ctx.AddCartItemAsync(ticket.Id, isSavedForLater: true);

        await using (var removeContext = ctx.NewContext())
        {
            var stored = await removeContext.CartItems.FirstAsync(
                x => x.PersonId == cartItem.PersonId && x.TicketId == cartItem.TicketId);
            stored.IsDeleted = true;
            await removeContext.SaveChangesAsync(CancellationToken.None);
        }

        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(
                new MoveCartItemToCartCommand { TicketId = ticket.Id },
                CancellationToken.None));
    }
}
