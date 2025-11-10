using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Events
{
    /// <summary>
    /// Represents a post that an organizer can make about an event
    /// </summary>
    public class EventNewsEntity : BaseEntity
    {
        /// <summary>
        /// Identifier of the person(s) resposible for creating this post
        /// </summary>
        public int OrganizerId { get; set; }
        /// <summary>
        /// The person(s) resposible for creating this post
        /// </summary>
        public OrganizerEntity Organizer { get; set; }
        /// <summary>
        /// The identifier of the event that this post concerns
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// The event that this post concerns
        /// </summary>
        public EventEntity Event { get; set; }
        /// <summary>
        /// Header text
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// Body text
        /// </summary>
        public string? Body { get; set; }
        /// <summary>
        /// Post image
        /// </summary>
        public byte[]? Image { get; set; }
    }
}
