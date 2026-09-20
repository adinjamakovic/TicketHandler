using Market.Application.Modules.Sales.Cart.Commands.Clear;
using Market.Tests.Common;

namespace Market.Tests.CartTests.UnitTests;

public class ClearCartCommandHandlerTests
{
    private static ClearCartCommandHandler CreateHandler(
        CartTestContext ctx,
        FakeAppCurrentUser currentUser) => new(ctx.Db, currentUser);

    [Fact]
    public async Task Handle_EmptiesTheCartButKeepsSavedLines()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var inCart = await ctx.AddTicketAsync();
        var saved = await ctx.AddTicketAsync(ticketTypeId: CartTestContext.VipTicketTypeId);

        await ctx.AddCartItemAsync(inCart.Id);
        await ctx.AddCartItemAsync(saved.Id, quantity: 3, isSavedForLater: true);

        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        await handler.Handle(new ClearCartCommand(), CancellationToken.None);

        var clearedLine = await ctx.GetCartItemAsync(inCart.Id);
        var savedLine = await ctx.GetCartItemAsync(saved.Id);

        Assert.True(clearedLine!.IsDeleted);
        Assert.False(savedLine!.IsDeleted);
        Assert.True(savedLine.IsSavedForLater);
        Assert.Equal(3, savedLine.Quantity);
    }

    [Fact]
    public async Task Handle_WhenOnlySavedLinesExist_ChangesNothing()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        await ctx.AddCartItemAsync(ticket.Id, isSavedForLater: true);

        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        await handler.Handle(new ClearCartCommand(), CancellationToken.None);

        var savedLine = await ctx.GetCartItemAsync(ticket.Id);

        Assert.False(savedLine!.IsDeleted);
        Assert.True(savedLine.IsSavedForLater);
    }
}
