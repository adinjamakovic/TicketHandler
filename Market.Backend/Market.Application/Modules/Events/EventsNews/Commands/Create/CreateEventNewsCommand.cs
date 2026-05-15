using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Commands.Create
{
    public class CreateEventNewsCommand : IRequest<int>
    {
        public required int EventId { get; set; }
        public required string Header { get; set; }
        public string? Body { get; set; }
        //public byte[] Image { get; set; }
    }
}
