using Market.Domain.Common;
using Market.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Sales
{
    /// <summary>
    /// Identifies tickets for events
    /// </summary>
    public class TicketsEntity : BaseEntity
    {
        /// <summary>
        /// Identifier for the event that the ticket pertains to
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// Event that the ticket pertains to
        /// </summary>
        public EventEntity Event { get; set; }
        /// <summary>
        /// Ticket type identifier
        /// </summary>
        public int TicketTypeId { get; set; }
        /// <summary>
        /// Ticket type
        /// </summary>
        public TicketTypeEntity TicketType { get; set; }
        /// <summary>
        /// Available quantity
        /// </summary>
        public decimal QuanityInStock { get; set; }
        /// <summary>
        /// Price of a single ticket
        /// </summary>
        public decimal UnitPrice {  get; set; }
        /// <summary>
        /// Benefits that this ticket grants the holder
        /// </summary>
        public string Benefits { get; set; }
    }
}
