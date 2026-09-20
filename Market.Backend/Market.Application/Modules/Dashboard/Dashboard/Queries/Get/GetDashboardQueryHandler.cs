using Market.Application.Modules.Sales.Orders;

namespace Market.Application.Modules.Dashboard.Dashboard.Query.Get;

public sealed class GetDashboardQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<GetDashboardQuery, GetDashboardQueryDto>
{
    public async Task<GetDashboardQueryDto> Handle(GetDashboardQuery req, CancellationToken ct)
    {
        if (!currentUser.IsAdmin)
            throw new MarketBusinessRuleException("121", "Only an admin can view the dashboard");

        // Only orders whose payment actually settled are money we took — a draft, an
        // abandoned checkout or an order held for review must not show up as revenue.
        // Held in a local so EF folds it into the query as a subselect.
        var settledOrderIds = OrderSettlement.SettledOrderIds(ctx);

        var soldItems = ctx.OrderItems.Where(x => settledOrderIds.Contains(x.OrderId));

        var dto = new GetDashboardQueryDto()
        {
            UserCount = await ctx.Persons.Where(x => !x.IsOrganiser && !x.IsAdmin).CountAsync(ct),
            OrganizerCount = await ctx.Organizers.CountAsync(ct),
            EventCount = await ctx.Events.CountAsync(ct),
            PerformerCount = await ctx.Performers.CountAsync(ct),
            TicketSales = await soldItems.Select(x => x.Quantity).SumAsync(ct),
            Revenue = await soldItems.Select(x => x.Total).SumAsync(ct)
        };
        
        return dto;
    }
}