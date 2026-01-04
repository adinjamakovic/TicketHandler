using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Commands.Update
{
    public class UpdateEventNewsCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        //public byte[] Image { get; set; }
    }
}
