namespace Market.Application.Modules.Events.Events.Queries.GetByOrganizerId;

public sealed class GetEventsByOrganizerIdQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required DateTime ScheduledDate { get; init; }
    public required string OrganizerName { get; init; }
    public required string VenueName { get; init; }
    public string? Image { get; set; }
    public required string EventTypeName { get; init; }
    public required List<GetEventsByOrganizerIdQueryDtoPerformers> Performers { get; init; }
}

public sealed class GetEventsByOrganizerIdQueryDtoPerformers
{
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required string Genre { get; init; }
}
