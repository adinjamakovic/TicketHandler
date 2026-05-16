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
        public ListTicketsQueryEvent Event { get; set; }
        public ListTicketsQueryTicketType TicketType { get; set; }
        public decimal QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public string Benefits { get; set; }
    }

    public class ListTicketsQueryEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ListTicketsQueryTicketType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
