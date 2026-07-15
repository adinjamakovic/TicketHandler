using Microsoft.AspNetCore.Http;

namespace Market.Application.Modules.Events.EventsNews.Commands.Update
{
    public class UpdateEventNewsCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public IFormFile? Image { get; set; }
    }
}
