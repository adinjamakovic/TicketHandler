using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Commands.Create
{
    public class CreateEventCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime ScheduledDate {  get; set; }
        public int VenueId { get; set; }
        //Fix later
        //public byte[] Image { get; set; }
        public int EventTypeId { get; set; }
        public List<CreateEventCommandPerformer> Performers { get; set; }
    }

    public class CreateEventCommandPerformer
    {
        public int PerformerId { get; set; }
        public TimeOnly TimeStamp { get; set; }
    }
}
