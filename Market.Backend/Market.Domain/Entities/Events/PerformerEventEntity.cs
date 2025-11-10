using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Events
{
    /// <summary>
    /// Identifies the link between performers and events
    /// </summary>
    public class PerformerEventEntity : BaseEntity
    {
        /// <summary>
        /// Identifier for the event
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// Identifies the performer 
        /// </summary>
        public int PerformerId { get; set; }
        /// <summary>
        /// Event performer
        /// </summary>
        public PerformerEntity Performer { get; set; }
        /// <summary>
        /// When the performer is performing
        /// </summary>
        public TimeOnly TimeStamp { get; set; }
    }
}
