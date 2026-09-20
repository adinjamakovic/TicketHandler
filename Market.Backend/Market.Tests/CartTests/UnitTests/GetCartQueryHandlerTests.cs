using Market.Application.Modules.Sales.Cart.Queries.Get;
using Market.Tests.Common;

namespace Market.Tests.CartTests.UnitTests;

public class GetCartQueryHandlerTests
{
    private static GetCartQueryHandler CreateHandler(
        CartTestContext ctx,
        FakeAppCurrentUser currentUser) => new(ctx.Db, currentUser, new FakeImageStorage());

    [Fact]
    public async Task Handle_SplitsCartLinesFromSavedLines()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var inCart = await ctx.AddTicketAsync(unitPrice: 50);
        var saved = await ctx.AddTicketAsync(ticketTypeId: CartTestContext.VipTicketTypeId, unitPrice: 120);

        await ctx.AddCartItemAsync(inCart.Id, quantity: 2);
        await ctx.AddCartItemAsync(saved.Id, quantity: 3, isSavedForLater: true);

        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        var dto = await handler.Handle(new GetCartQuery(), CancellationToken.None);

        Assert.Equal(inCart.Id, Assert.Single(dto.Items).TicketId);
        Assert.Equal(saved.Id, Assert.Single(dto.SavedItems).TicketId);
        Assert.Equal(1, dto.LineCount);
        Assert.Equal(1, dto.SavedLineCount);
        Assert.False(dto.Items[0].IsSavedForLater);
        Assert.True(dto.SavedItems[0].IsSavedForLater);
    }

    [Fact]
    public async Task Handle_KeepsSavedLinesOutOfTheTotals()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var inCart = await ctx.AddTicketAsync(unitPrice: 50);
        var saved = await ctx.AddTicketAsync(ticketTypeId: CartTestContext.VipTicketTypeId, unitPrice: 120);

        await ctx.AddCartItemAsync(inCart.Id, quantity: 2);
        await ctx.AddCartItemAsync(saved.Id, quantity: 3, isSavedForLater: true);

        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        var dto = await handler.Handle(new GetCartQuery(), CancellationToken.None);

        // 2 x 50 only — the 3 x 120 sitting on the shelf is priced but not counted.
        Assert.Equal(100m, dto.TotalAmount);
        Assert.Equal(2m, dto.TotalQuantity);
        Assert.Equal(360m, dto.SavedItems[0].Subtotal);
    }

    [Fact]
    public async Task Handle_WhenEverythingIsSaved_ReportsAnEmptyCart()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(unitPrice: 40);
        await ctx.AddCartItemAsync(ticket.Id, quantity: 2, isSavedForLater: true);

        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        var dto = await handler.Handle(new GetCartQuery(), CancellationToken.None);

        Assert.Empty(dto.Items);
        Assert.Equal(0, dto.LineCount);
        Assert.Equal(0m, dto.TotalAmount);
        Assert.Single(dto.SavedItems);
    }

    [Fact]
    public async Task Handle_DoesNotReturnAnotherPersonsSavedLines()
    {
        await using var ctx = await CartTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        await ctx.AddCartItemAsync(ticket.Id, personId: CartTestContext.OtherBuyerId, isSavedForLater: true);

        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(CartTestContext.BuyerId));

        var dto = await handler.Handle(new GetCartQuery(), CancellationToken.None);

        Assert.Empty(dto.Items);
        Assert.Empty(dto.SavedItems);
    }
}
