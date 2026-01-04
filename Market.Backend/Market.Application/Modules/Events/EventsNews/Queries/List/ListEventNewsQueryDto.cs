using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Queries.List
{
    public class ListEventNewsQueryDto
    {
        public int Id { get; set; }
        public string Event { get; set; }
        public string Organizer { get; set; }
        public string Header { get; set; }
        public string? Body { get; set; }
        public byte[] Image { get; set; }
    }

}
