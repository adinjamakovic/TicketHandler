using Microsoft.AspNetCore.Http;

namespace Market.Application.Modules.Events.Events.Commands.Create;

public class CreateEventCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int VenueId { get; set; }
    public IFormFile? Image { get; set; }
    public int EventTypeId { get; set; }
    public List<CreateEventCommandPerformer> Performers { get; set; } = [];
}

public class CreateEventCommandPerformer
{
    public int PerformerId { get; set; }
    public TimeOnly TimeStamp { get; set; }
}
