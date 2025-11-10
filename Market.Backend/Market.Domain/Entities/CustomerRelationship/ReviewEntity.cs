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
    /// Represents a review of a certain event
    /// </summary>
    public class ReviewEntity : BaseEntity
    {
        /// <summary>
        /// Identifier of the person who submited the review
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// The person who submited the review
        /// </summary>
        public PersonEntity Person { get; set; }
        /// <summary>
        /// The identifier of the order linked with the review
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// The order linked with the review
        /// </summary>
        public OrderEntity Order { get; set; }
        /// <summary>
        /// Event grade
        /// </summary>
        public int EventRating { get; set; }
        /// <summary>
        /// Organizer grade
        /// </summary>
        public int OrganizerRating { get; set; }
        /// <summary>
        /// Performer grade
        /// </summary>
        public int PerformerRating { get; set; }
        /// <summary>
        /// Event review
        /// </summary>
        public string EventReview {  get; set; }
        /// <summary>
        /// Organizer review
        /// </summary>
        public string OrganizerReview { get; set; }
        /// <summary>
        /// Performer review
        /// </summary>
        public string PerformerReview { get; set; }
    }
}
