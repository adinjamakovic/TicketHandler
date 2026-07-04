namespace Market.Application.Modules.Events.Events.Queries.List;

public class ListEventsQuery : BasePagedQuery<ListEventsQueryDto>
{
    public string? Search { get; set; }
    public DateTime? Date { get; set; }
    public string? City { get; set; }
}
