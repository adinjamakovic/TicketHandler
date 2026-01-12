using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.ListWithTicketsAndEvents
{
    public sealed class ListOrdersWithTicketsAndEventsQueryDto
    {
        public required int Id { get; init; }
        public required int PersonId { get; init; }
        public required List<ListOrderOrderItemsWithTicketsAndEvents> Items { get; init; }
    }

    public class ListOrderOrderItemsWithTicketsAndEvents
    {
        public int Id { get; init; }
        public int OrderId { get; init; }
        public ListOrdersWithEventsQueryDtoTickets Ticket { get; init; }
        public decimal Quantity { get; init; }
        public decimal Subtotal { get; init; }
        public decimal? DiscountAmount { get; init; }
        public decimal? DiscountPercent { get; init; }
        public decimal Total { get; init; }
    }

    public class ListOrdersWithEventsQueryDtoTickets
    {
        public int Id { get; init; }
        public ListOrdersQueryDtoEvent Event { get; set; }
        public int TicketTypeId { get; set; }
        public decimal QuanityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public string Benefits { get; set; }
    }

    public class ListOrdersQueryDtoEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int OrganizerId { get; set; }
        public int VenueId { get; set; }
        public int EventTypeId { get; set; }
    }
}
