using Market.Application.Common.Exceptions;
using Market.Application.Modules.Dashboard.Dashboard.Query.Get;
using Market.Domain.Entities.CustomerRelationship;
using Market.Tests.Common;
using Market.Tests.PaymentTests.UnitTests;

namespace Market.Tests.DashboardTests.UnitTests;

// The dashboard reads the same settled status settlement writes, so what it reports as
// revenue is what was actually collected — nothing else.
public class GetDashboardQueryHandlerTests
{
    private static GetDashboardQueryHandler CreateHandler(
        PaymentsTestContext ctx,
        FakeAppCurrentUser currentUser) => new(ctx.Db, currentUser);

    [Fact]
    public async Task Handle_CountsOnlyOrdersWhosePaymentSettled()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(unitPrice: 50);

        var paid = await ctx.AddOrderAsync((ticket.Id, 2m, 100m));
        await ctx.AddTransactionAsync(paid.Id, 100m, "pi_paid", OrderStatusType.Paid);

        var abandoned = await ctx.AddOrderAsync((ticket.Id, 5m, 250m));
        await ctx.AddTransactionAsync(abandoned.Id, 250m, "pi_draft", OrderStatusType.Draft);

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        var dto = await handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        Assert.Equal(2, dto.TicketSales);
        Assert.Equal(100m, dto.Revenue);
    }

    [Fact]
    public async Task Handle_LeavesOutOrdersHeldForReview()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(unitPrice: 50);

        var held = await ctx.AddOrderAsync((ticket.Id, 4m, 200m));
        await ctx.AddTransactionAsync(held.Id, 200m, "pi_review", OrderStatusType.PaymentReview);

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        var dto = await handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        Assert.Equal(0, dto.TicketSales);
        Assert.Equal(0m, dto.Revenue);
    }

    [Fact]
    public async Task Handle_LeavesOutOrdersWithNoPaymentAtAll()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(unitPrice: 50);
        await ctx.AddOrderAsync((ticket.Id, 3m, 150m));

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        var dto = await handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        Assert.Equal(0, dto.TicketSales);
        Assert.Equal(0m, dto.Revenue);
    }

    [Fact]
    public async Task Handle_WhenTheCallerIsNotAnAdmin_IsRefused()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();

        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(PaymentsTestContext.BuyerId));

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(new GetDashboardQuery(), CancellationToken.None));
    }
}
