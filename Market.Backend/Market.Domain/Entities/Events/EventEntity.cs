using Market.Domain.Common;
using Market.Domain.Entities.CustomerRelationship;
using Market.Domain.Entities.Geographical;
using Market.Domain.Entities.Sales;
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
        public ICollection<EventNewsEntity> EventNews { get; set; } = new List<EventNewsEntity>();
        public ICollection<PerformerEventEntity> PerformerEvents { get; set; } = new List<PerformerEventEntity>();
        public ICollection<TicketsEntity> Tickets { get; set; } = new List<TicketsEntity>();
        public ICollection<ReviewEntity> Reviews { get; set; } = new List<ReviewEntity>();
    }
}
