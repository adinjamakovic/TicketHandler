using Market.Domain.Common;
using Market.Domain.Entities.Identity;
using Market.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.CustomerRelationship
{
    /// <summary>
    /// Used for transaction history
    /// </summary>
    public class TransactionEntity : BaseEntity
    {
        /// <summary>
        /// Date when the transaction was made
        /// </summary>
        public DateTime? PaidAt { get; set; }
        /// <summary>
        /// Transaction status in reference of the order
        /// </summary>
        public OrderStatusType Status { get; set; }
        /// <summary>
        /// Total amount of the transaction in the default currency.
        /// </summary>
        public required decimal TotalAmount { get; set; }
        /// <summary>
        /// Identifier of the person that paid the amount
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// The person that paid the amount
        /// </summary>
        public PersonEntity Person { get; set; }
        /// <summary>
        /// Identifier of the order that this transaction pertains to
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// The order that this transaction pertains to
        /// </summary>
        public OrderEntity Order { get; set; }
        /// <summary>
        /// Used for stripe payment
        /// </summary>
        public string StripeToken { get; set; }
    }
}
