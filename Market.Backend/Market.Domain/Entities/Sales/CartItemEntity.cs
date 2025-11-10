using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Sales
{
    /// <summary>
    /// Identifies a singular cart item
    /// </summary>
    public class CartItemEntity : BaseEntity
    {
        /// <summary>
        /// Identifier for the ticket that is a cart item
        /// </summary>
        public int TicketId { get; set; }
        /// <summary>
        /// The ticket that is a cart item
        /// </summary>
        public TicketsEntity Ticket {  get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        public decimal Quantity { get; set; }
    }
}
