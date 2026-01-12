using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.ListWithTickets
{
    public sealed class ListOrdersWithTicketsQueryDto
    {
        public required int Id { get; init; }
        public required int PersonId { get; init; }
        public required List<ListOrderOrderItemsWithTickets> Items { get; init; }
    }

    public class ListOrderOrderItemsWithTickets
    {
        public int Id { get; init; }
        public int OrderId { get; init; }
        public ListOrdersQueryDtoTickets Ticket { get; init; }
        public decimal Quantity { get; init; }
        public decimal Subtotal { get; init; }
        public decimal? DiscountAmount { get; init; }
        public decimal? DiscountPercent { get; init; }
        public decimal Total { get; init; }
    }

    public class ListOrdersQueryDtoTickets
    {
        public int Id { get; init; }
        public int EventId { get; set; }
        public int TicketTypeId { get; set; }
        public decimal QuanityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public string Benefits { get; set; }
    }
}
