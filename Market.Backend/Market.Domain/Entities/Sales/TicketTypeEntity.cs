using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Sales
{
    /// <summary>
    /// Identifies the ticket type
    /// </summary>
    public class TicketTypeEntity : BaseEntity
    {
        /// <summary>
        /// Ticket type name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ticket type description
        /// </summary>
        public string Description { get; set; }
    }
}
