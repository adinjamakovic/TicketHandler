namespace Market.Application.Modules.Events.Events.Queries.List;

public class ListEventsQuery : BasePagedQuery<ListEventsQueryDto>
{
    public string? Search { get; set; }
}
