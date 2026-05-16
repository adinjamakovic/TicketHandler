using Microsoft.AspNetCore.Http;

namespace Market.Application.Modules.Events.Events.Commands.Update
{
    public class UpdateEventCommand : IRequest<int>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int VenueId { get; set; }
        public IFormFile? Image { get; set; }
        public int EventTypeId { get; set; }
        public List<UpdateEventCommandPerformers> Performers { get; set; } = [];
    }

    public class UpdateEventCommandPerformers
    {
        public int Id { get; set; }
        public int PerformerId { get; set; }
        public TimeOnly TimeStamp { get; set; }
    }
}
