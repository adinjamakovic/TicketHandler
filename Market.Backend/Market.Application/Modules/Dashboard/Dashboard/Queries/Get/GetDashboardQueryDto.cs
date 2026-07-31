namespace Market.Application.Modules.Dashboard.Dashboard.Query.Get;

public class GetDashboardQueryDto
{
    public int UserCount { get; set; }
    public int OrganizerCount { get; set; }
    public int EventCount { get; set; }
    public int  PerformerCount { get; set; }
    public decimal TicketSales { get; set; }
    public decimal Revenue { get; set; }
}