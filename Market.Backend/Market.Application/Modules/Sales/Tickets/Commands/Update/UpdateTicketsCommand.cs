using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Commands.Update
{
    public class UpdateTicketsCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public int EventId { get; set; }
        public int TicketTypeId { get; set; }
        public decimal QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public string Benefits { get; set; }
    }
}
