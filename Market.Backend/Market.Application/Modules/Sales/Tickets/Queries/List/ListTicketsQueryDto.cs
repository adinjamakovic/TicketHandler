using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Queries.List
{
    public class ListTicketsQueryDto
    {
        public required int Id { get; init; }
        public int EventId { get; set; }
        public int TicketTypeId { get; set; }
        public decimal QuanityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public string Benefits { get; set; }
    }

}
