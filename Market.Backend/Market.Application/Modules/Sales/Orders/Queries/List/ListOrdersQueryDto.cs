using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.List
{
    public sealed class ListOrdersQueryDto
    {
        public required int Id { get; init; }
        public required int PersonId { get; init; }
        public required List<ListOrderOrderItems> Items { get; init; }
    }

    public class ListOrderOrderItems
    {
        public int Id { get; init; }
        public int OrderId { get; init; }
        public int TicketId { get; init; }
        public decimal Quantity { get; init; }
        public decimal Subtotal { get; init; }
        public decimal? DiscountAmount { get; init; }
        public decimal? DiscountPercent { get; init; }
        public decimal Total { get; init; }
    }

}
