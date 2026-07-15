namespace Market.Application.Modules.Events.Events.Queries.List;

public class ListEventsQuery : BasePagedQuery<ListEventsQueryDto>
{
    public string? Search { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? City { get; set; }
}
