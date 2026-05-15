using Market.Application.Modules.Events.Events.Queries.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Queries.GetById
{
    public sealed class GetTicketsByIdQueryDto
    {
        public required int Id { get; init; }
        public GetTicketsByIdQueryDtoEvent Event { get; set; }
        public GetTicketsByIdQueryDtoTicketType TicketType { get; set; }
        public decimal QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public string Benefits { get; set; }
    }

    public class GetTicketsByIdQueryDtoTicketType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GetTicketsByIdQueryDtoEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
