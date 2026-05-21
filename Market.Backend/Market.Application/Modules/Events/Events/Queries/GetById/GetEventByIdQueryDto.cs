namespace Market.Application.Modules.Events.Events.Queries.GetById;

public sealed class GetEventByIdQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required DateTime ScheduledDate { get; init; }
    public required string OrganizerName { get; init; }
    public required string VenueName { get; init; }
    /// <summary>Web path for static file serving, e.g. /Upload/Events/{file}. Null when no image.</summary>
    public string? Image { get; set; }
    public required string EventTypeName { get; init; }
    public required List<GetEventByIdQueryDtoPerformers> Performers { get; init; }
}

public sealed class GetEventByIdQueryDtoPerformers
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required string Genre { get; init; }
}
