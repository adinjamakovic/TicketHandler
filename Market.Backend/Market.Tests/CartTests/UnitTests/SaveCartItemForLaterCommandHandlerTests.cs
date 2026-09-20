using Market.Application.Common.Exceptions;
using Market.Application.Modules.Sales.Cart.Commands.SaveForLater;
using Market.Tests.Common;

namespace Market.Tests.CartTests.UnitTests;

public class SaveCartItemForLaterCommandHandlerTests
{
    private static SaveCartItemForLaterCommandHandler CreateHandler(
        CartTestContext ctx,
        FakeAppCurrentUser currentUser) => new(ctx.Db, currentUser);

    [Fact]
    public async Task Handle_OnCartLine_ParksItWithoutLosingTheQuantity()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        await ctx.AddCartItemAsync(ticket.Id, quantity: 3);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        await handler.Handle(new SaveCartItemForLaterCommand { TicketId = ticket.Id }, CancellationToken.None);

        var stored = await ctx.GetCartItemAsync(ticket.Id);

        Assert.True(stored!.IsSavedForLater);
        Assert.Equal(3, stored.Quantity);
        Assert.False(stored.IsDeleted);
    }

    [Fact]
    public async Task Handle_WhenAlreadySaved_IsANoOp()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        await ctx.AddCartItemAsync(ticket.Id, quantity: 2, isSavedForLater: true);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        await handler.Handle(new SaveCartItemForLaterCommand { TicketId = ticket.Id }, CancellationToken.None);

        var stored = await ctx.GetCartItemAsync(ticket.Id);

        Assert.True(stored!.IsSavedForLater);
        Assert.Equal(2, stored.Quantity);
    }

    [Fact]
    public async Task Handle_WhenTicketIsNotInTheCart_ThrowsNotFound()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        var ex = await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(
                new SaveCartItemForLaterCommand { TicketId = CartTestContext.MissingId },
                CancellationToken.None));

        Assert.Equal($"Ticket with Id {CartTestContext.MissingId} is not in your cart", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenTheLineBelongsToAnotherPerson_ThrowsNotFound()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        await ctx.AddCartItemAsync(ticket.Id, personId: CartTestContext.OtherBuyerId);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(
                new SaveCartItemForLaterCommand { TicketId = ticket.Id },
                CancellationToken.None));

        // The other person's line is untouched.
        var stored = await ctx.GetCartItemAsync(ticket.Id, CartTestContext.OtherBuyerId);
        Assert.False(stored!.IsSavedForLater);
    }

    [Fact]
    public async Task Handle_WhenCallerIsAnonymous_ThrowsBusinessRule()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        await ctx.AddCartItemAsync(ticket.Id);
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Anonymous());

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(
                new SaveCartItemForLaterCommand { TicketId = ticket.Id },
                CancellationToken.None));

        Assert.Equal("CART_NO_USER", ex.Code);
    }
}
