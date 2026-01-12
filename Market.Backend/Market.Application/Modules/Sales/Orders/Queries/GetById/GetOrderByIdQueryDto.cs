using Market.Application.Modules.Events.Events.Queries.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.GetById
{
    public sealed class GetOrderByIdQueryDto
    {
        public required int Id { get; init; }
        public required int PersonId { get; init; }
        public required List<GetOrderByIdOrderItems> Items { get; init; }
    }

    public class GetOrderByIdOrderItems
    {
        public int Id { get; init; }
        public int OrderId { get; init; }
        public GetOrderByIdQueryDtoTicket Ticket { get; init; }
        public decimal Quantity { get; init; }
        public decimal Subtotal { get; init; }
        public decimal? DiscountAmount { get; init; }
        public decimal? DiscountPercent { get; init; }
        public decimal Total { get; init; }
    }

    public sealed class GetOrderByIdQueryDtoTicket
    {
        public int Id { get; init; }
        public int EventId { get; set; }
        public int TicketTypeId { get; set; }
        public decimal QuanityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public string Benefits { get; set; }
    }
}
