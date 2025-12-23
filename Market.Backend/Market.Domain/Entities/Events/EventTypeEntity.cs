using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Events
{
    /// <summary>
    /// Represents an event type
    /// </summary>
    public class EventTypeEntity : BaseEntity
    {
        /// <summary>
        /// Event type name
        /// </summary>
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public ICollection<EventEntity> Events { get; set; } = new List<EventEntity>();
    }
}
