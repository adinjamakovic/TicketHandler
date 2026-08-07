using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Queries.Get
{
    public sealed class GetCartQueryDto
    {
        public List<GetCartQueryDtoItem> Items { get; set; } = new();
        public int LineCount { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public sealed class GetCartQueryDtoItem
    {
        public int TicketId { get; set; }
        public GetCartQueryDtoEvent Event { get; set; }
        public GetCartQueryDtoTicketType TicketType { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal QuantityInStock { get; set; }
        public string Benefits { get; set; }
        public DateTime AddedAtUtc { get; set; }
    }

    public sealed class GetCartQueryDtoEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string VenueName { get; set; }
        public string Image { get; set; }
    }

    public sealed class GetCartQueryDtoTicketType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
