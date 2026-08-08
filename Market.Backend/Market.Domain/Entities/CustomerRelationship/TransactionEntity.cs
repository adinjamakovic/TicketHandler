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
        /// Id of the Stripe PaymentIntent this transaction settles (pi_...).
        /// Every payment attempt is looked up by this value, both when the browser
        /// reports back and when the Stripe webhook arrives.
        /// </summary>
        public string StripeToken { get; set; }
        /// <summary>
        /// Concurrency token. The browser and the Stripe webhook can report the same
        /// successful payment at the same time; this makes sure only one of them gets to
        /// take the tickets out of stock.
        /// </summary>
        public byte[]? RowVersion { get; set; }
    }
}
