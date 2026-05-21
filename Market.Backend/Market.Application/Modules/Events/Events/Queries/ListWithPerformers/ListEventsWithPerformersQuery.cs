namespace Market.Application.Modules.Events.Events.Queries.ListWithPerformers;

public sealed class ListEventsWithPerformersQuery : BasePagedQuery<ListEventsWithPerformersQueryDto>
{
    public string? Search { get; init; }
}
