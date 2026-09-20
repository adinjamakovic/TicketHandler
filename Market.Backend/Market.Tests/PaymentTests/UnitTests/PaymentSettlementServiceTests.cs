using Market.Application.Abstractions.Payments;
using Market.Application.Common.Exceptions;
using Market.Application.Modules.Sales.Payments;
using Market.Domain.Entities.CustomerRelationship;
using Market.Tests.Common;
using Microsoft.Extensions.Logging.Abstractions;

namespace Market.Tests.PaymentTests.UnitTests;

// Settlement is the only place tickets leave stock and the only place an order becomes Paid.
// These cover the cases where the payment succeeded but the order cannot honestly follow.
public class PaymentSettlementServiceTests
{
    private const string IntentId = "pi_test_1";

    private static PaymentSettlementService CreateService(
        PaymentsTestContext ctx,
        FakePaymentGateway gateway) =>
        new(ctx.Db, gateway, NullLogger<PaymentSettlementService>.Instance);

    [Fact]
    public async Task SettleAsync_WhenPaymentSucceeds_MarksPaidTakesStockAndClearsTheCart()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(quantityInStock: 10, unitPrice: 50);
        var order = await ctx.AddOrderAsync((ticket.Id, 2m, 100m));
        await ctx.AddTransactionAsync(order.Id, 100m, IntentId);
        await ctx.AddCartItemAsync(ticket.Id, quantity: 2);

        var service = CreateService(ctx, FakePaymentGateway.Succeeded(IntentId, 100m));

        var result = await service.SettleAsync(IntentId, PaymentsTestContext.BuyerId, CancellationToken.None);

        Assert.True(result.IsPaid);
        Assert.False(result.RequiresReview);

        var transaction = await ctx.GetTransactionAsync(IntentId);
        Assert.Equal(OrderStatusType.Paid, transaction.Status);
        Assert.NotNull(transaction.PaidAt);
        Assert.Null(transaction.SettlementIssue);

        Assert.Equal(8, (await ctx.GetTicketAsync(ticket.Id)).QuantityInStock);
        Assert.True((await ctx.GetCartItemAsync(ticket.Id))!.IsDeleted);
    }

    [Fact]
    public async Task SettleAsync_WhenTheAmountPaidDoesNotMatchTheOrder_HoldsForReviewAndLeavesStockAlone()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(quantityInStock: 10, unitPrice: 50);
        var order = await ctx.AddOrderAsync((ticket.Id, 2m, 100m));
        await ctx.AddTransactionAsync(order.Id, 100m, IntentId);
        await ctx.AddCartItemAsync(ticket.Id, quantity: 2);

        // The provider took 40 for an order that costs 100.
        var service = CreateService(ctx, FakePaymentGateway.Succeeded(IntentId, 40m));

        var result = await service.SettleAsync(IntentId, PaymentsTestContext.BuyerId, CancellationToken.None);

        Assert.False(result.IsPaid);
        Assert.True(result.RequiresReview);

        var transaction = await ctx.GetTransactionAsync(IntentId);
        Assert.Equal(OrderStatusType.PaymentReview, transaction.Status);
        Assert.Contains("40", transaction.SettlementIssue!);
        // The charge is still a fact, even though nothing was handed over.
        Assert.NotNull(transaction.PaidAt);

        Assert.Equal(10, (await ctx.GetTicketAsync(ticket.Id)).QuantityInStock);
        Assert.False((await ctx.GetCartItemAsync(ticket.Id))!.IsDeleted);
    }

    [Fact]
    public async Task SettleAsync_WhenStockRanOut_HoldsForReviewRatherThanOverselling()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(quantityInStock: 1, unitPrice: 50);
        var order = await ctx.AddOrderAsync((ticket.Id, 3m, 150m));
        await ctx.AddTransactionAsync(order.Id, 150m, IntentId);

        var service = CreateService(ctx, FakePaymentGateway.Succeeded(IntentId, 150m));

        var result = await service.SettleAsync(IntentId, PaymentsTestContext.BuyerId, CancellationToken.None);

        Assert.True(result.RequiresReview);

        var transaction = await ctx.GetTransactionAsync(IntentId);
        Assert.Equal(OrderStatusType.PaymentReview, transaction.Status);
        Assert.Contains("Cannot fulfil", transaction.SettlementIssue!);

        // Stock is not quietly floored to zero — it stays as it was for whoever sorts this out.
        Assert.Equal(1, (await ctx.GetTicketAsync(ticket.Id)).QuantityInStock);
    }

    [Fact]
    public async Task SettleAsync_WhenTwoLinesShareATicket_ChecksTheirTotalAgainstStock()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(quantityInStock: 3, unitPrice: 50);
        var order = await ctx.AddOrderAsync((ticket.Id, 2m, 100m), (ticket.Id, 2m, 100m));
        await ctx.AddTransactionAsync(order.Id, 200m, IntentId);

        var service = CreateService(ctx, FakePaymentGateway.Succeeded(IntentId, 200m));

        var result = await service.SettleAsync(IntentId, PaymentsTestContext.BuyerId, CancellationToken.None);

        Assert.True(result.RequiresReview);
        Assert.Equal(3, (await ctx.GetTicketAsync(ticket.Id)).QuantityInStock);
    }

    [Fact]
    public async Task SettleAsync_WhenThePaidOrderHasNoItems_HoldsForReview()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var order = await ctx.AddOrderAsync();
        await ctx.AddTransactionAsync(order.Id, 100m, IntentId);

        var service = CreateService(ctx, FakePaymentGateway.Succeeded(IntentId, 100m));

        var result = await service.SettleAsync(IntentId, PaymentsTestContext.BuyerId, CancellationToken.None);

        Assert.True(result.RequiresReview);
        Assert.Equal(OrderStatusType.PaymentReview, (await ctx.GetTransactionAsync(IntentId)).Status);
    }

    [Fact]
    public async Task SettleAsync_WhenAlreadyPaid_DoesNotTakeStockTwice()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(quantityInStock: 10, unitPrice: 50);
        var order = await ctx.AddOrderAsync((ticket.Id, 2m, 100m));
        await ctx.AddTransactionAsync(order.Id, 100m, IntentId);

        var gateway = FakePaymentGateway.Succeeded(IntentId, 100m);

        // The browser and the webhook both report the same payment.
        await CreateService(ctx, gateway).SettleAsync(IntentId, PaymentsTestContext.BuyerId, CancellationToken.None);

        await using var secondContext = ctx.NewContext();
        var second = new PaymentSettlementService(
            secondContext, gateway, NullLogger<PaymentSettlementService>.Instance);

        var result = await second.SettleAsync(IntentId, null, CancellationToken.None);

        Assert.True(result.IsPaid);
        Assert.Equal(8, (await ctx.GetTicketAsync(ticket.Id)).QuantityInStock);
    }

    [Fact]
    public async Task SettleAsync_WhenHeldForReview_DoesNotRunFulfilmentAgain()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(quantityInStock: 10, unitPrice: 50);
        var order = await ctx.AddOrderAsync((ticket.Id, 2m, 100m));
        await ctx.AddTransactionAsync(
            order.Id, 100m, IntentId, status: OrderStatusType.PaymentReview);

        // A retrying webhook must not take over from whoever is handling the order.
        var service = CreateService(ctx, FakePaymentGateway.Succeeded(IntentId, 100m));

        var result = await service.SettleAsync(IntentId, null, CancellationToken.None);

        Assert.True(result.RequiresReview);
        Assert.Equal(10, (await ctx.GetTicketAsync(ticket.Id)).QuantityInStock);
    }

    [Fact]
    public async Task SettleAsync_WhenTheIntentWasCanceled_CancelsTheOrderAndTakesNoStock()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync(quantityInStock: 10, unitPrice: 50);
        var order = await ctx.AddOrderAsync((ticket.Id, 2m, 100m));
        await ctx.AddTransactionAsync(order.Id, 100m, IntentId);

        var gateway = FakePaymentGateway.Reporting(IntentId, PaymentIntentStatus.Canceled, 100m);

        var result = await CreateService(ctx, gateway)
            .SettleAsync(IntentId, PaymentsTestContext.BuyerId, CancellationToken.None);

        Assert.False(result.IsPaid);
        Assert.False(result.RequiresReview);
        Assert.Equal(OrderStatusType.Cancelled, (await ctx.GetTransactionAsync(IntentId)).Status);
        Assert.Equal(10, (await ctx.GetTicketAsync(ticket.Id)).QuantityInStock);
    }

    [Fact]
    public async Task SettleAsync_WhenTheIntentBelongsToSomeoneElse_IsNotFound()
    {
        await using var ctx = await PaymentsTestContext.CreateAsync();
        var ticket = await ctx.AddTicketAsync();
        var order = await ctx.AddOrderAsync((ticket.Id, 1m, 50m));
        await ctx.AddTransactionAsync(order.Id, 50m, IntentId);

        var service = CreateService(ctx, FakePaymentGateway.Succeeded(IntentId, 50m));

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => service.SettleAsync(IntentId, expectedPersonId: 999, CancellationToken.None));
    }
}
