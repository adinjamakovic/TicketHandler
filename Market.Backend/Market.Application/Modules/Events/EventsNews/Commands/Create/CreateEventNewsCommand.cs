using Microsoft.AspNetCore.Http;

namespace Market.Application.Modules.Events.EventsNews.Commands.Create
{
    public class CreateEventNewsCommand : IRequest<int>
    {
        public required int EventId { get; set; }
        public required string Header { get; set; }
        public string? Body { get; set; }
        public IFormFile? Image { get; set; }
    }
}
