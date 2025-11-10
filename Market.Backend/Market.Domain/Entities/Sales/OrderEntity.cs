using Market.Domain.Common;
using Market.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Sales
{
    /// <summary>
    /// Identifies an Order
    /// </summary>
     public class OrderEntity : BaseEntity
    {
        /// <summary>
        /// Identifier for the person who ordered 
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// The person who ordered 
        /// </summary>
        public PersonEntity Person { get; set; }
        /// <summary>
        /// When the order was created
        /// </summary>
        public DateTime OrderedAtUtc { get; set; }
        
    }
}
