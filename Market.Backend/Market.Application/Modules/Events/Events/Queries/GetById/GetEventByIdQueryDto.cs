namespace Market.Application.Modules.Events.Events.Queries.GetById;

public sealed class GetEventByIdQueryDto
{
    public required int Id { get; init; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int VenueId { get; set; }
    public string VenueName { get; set; }
    public string? VenueCity { get; set; }
    public string? Image { get; set; }
    public int EventTypeId { get; set; }
    public string EventTypeName { get; set; }
    public string OrganizerName { get; set; }
    public List<GetEventByIdQueryDtoPerformers> Performers { get; set; } = [];
}

public sealed class GetEventByIdQueryDtoPerformers
{
    public int PerformerId { get; set; }
    public TimeOnly TimeStamp { get; set; }
}
