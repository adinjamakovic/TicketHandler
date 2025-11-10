using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Sales
{
    /// <summary>
    /// Identifies a singular order item
    /// </summary>
    public class OrderItemEntity : BaseEntity
    {
        /// <summary>
        /// Identifier of what order this item belongs to
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// The order that the item belongs to
        /// </summary>
        public OrderEntity Order { get; set; }
        /// <summary>
        /// Identifier for the bought ticket
        /// </summary>
        public int TicketId { get; set; }
        /// <summary>
        /// The bought ticket
        /// </summary>
        public TicketsEntity Ticket {  get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// Calculated subtotal (Quantity × UnitPrice).
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Discount applied to this item (optional).
        /// </summary>
        public decimal? DiscountAmount { get; set; }

        /// <summary>
        /// Discount percent to this item (optional).
        /// </summary>
        public decimal? DiscountPercent { get; set; }

        /// <summary>
        /// Final total for this item (Subtotal - Discount).
        /// </summary>
        public decimal Total { get; set; }
    }
}
