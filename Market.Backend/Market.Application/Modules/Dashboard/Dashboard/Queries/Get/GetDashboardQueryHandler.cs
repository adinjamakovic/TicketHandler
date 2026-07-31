namespace Market.Application.Modules.Dashboard.Dashboard.Query.Get;

public sealed class GetDashboardQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<GetDashboardQuery, GetDashboardQueryDto>
{
    public async Task<GetDashboardQueryDto> Handle(GetDashboardQuery req, CancellationToken ct)
    {
        if (!currentUser.IsAdmin)
            throw new MarketBusinessRuleException("121", "Only an admin can view the dashboard");

        var dto = new GetDashboardQueryDto()
        {
            UserCount = await ctx.Persons.Where(x => !x.IsOrganiser && !x.IsAdmin).CountAsync(ct),
            OrganizerCount = await ctx.Organizers.CountAsync(ct),
            EventCount = await ctx.Events.CountAsync(ct),
            PerformerCount = await ctx.Performers.CountAsync(ct),
            TicketSales = await ctx.OrderItems.Select(x => x.Quantity).SumAsync(ct),
            Revenue = await ctx.OrderItems.Select(x => x.Total).SumAsync(ct)
        };
        
        return dto;
    }
}