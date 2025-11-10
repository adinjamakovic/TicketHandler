using Market.Domain.Common;
using Market.Domain.Entities.Geographical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Events
{
    /// <summary>
    /// Represents an event
    /// </summary>
    public class EventEntity : BaseEntity
    {
        /// <summary>
        /// Event name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Event description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Date when the event will take place
        /// </summary>
        public DateTime ScheduledDate {  get; set; }
        /// <summary>
        /// Identifier of the person(s) resposible for organizing the event
        /// </summary>
        public int OrganizerId { get; set; }
        /// <summary>
        /// The person(s) resposible for organizing the event
        /// </summary>
        public OrganizerEntity Organizer { get; set; }
        /// <summary>
        /// Identifier of the venue where the event will take place
        /// </summary>
        public int VenueId { get; set; }
        /// <summary>
        /// Venue where the event will take place
        /// </summary>
        public VenueEntity Venue { get; set; }
        /// <summary>
        /// Event poster
        /// </summary>
        public byte[] Image {  get; set; }
        /// <summary>
        /// Event type identifier
        /// </summary>
        public int EventTypeId { get; set; }
        /// <summary>
        /// Event type
        /// </summary>
        public EventTypeEntity EventType { get; set; }
    }
}
