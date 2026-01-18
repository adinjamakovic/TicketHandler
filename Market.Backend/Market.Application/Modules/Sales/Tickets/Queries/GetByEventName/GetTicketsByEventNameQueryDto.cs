using Market.Application.Modules.Events.Events.Queries.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Queries.GetByEventName
{
    public sealed class GetTicketsByEventNameQueryDto
    {
        public required int Id { get; init; }
        public GetTicketsByEventNameQueryDtoEvent Event { get; set; }
        public GetTicketsByEventNameQueryDtoTicketType TicketType { get; set; }
        public decimal QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public string Benefits { get; set; }
    }

    public class GetTicketsByEventNameQueryDtoTicketType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GetTicketsByEventNameQueryDtoEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
