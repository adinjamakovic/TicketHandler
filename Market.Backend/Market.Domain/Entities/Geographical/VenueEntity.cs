using Market.Domain.Common;
using Market.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Geographical
{
    public class VenueEntity : BaseEntity
    {
        /// <summary>
        /// Venue name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Number of seated people the venue can hold 
        /// </summary>
        public int Seated { get; set; }
        /// <summary>
        /// Number of standing people the venue can hold
        /// </summary>
        public int Standing { get; set; }
        /// <summary>
        /// Identifier of venue location
        /// </summary>
        public int LocationId { get; set; }
        /// <summary>
        /// Venue Location
        /// </summary>
        public LocationEntity Location { get; set; }
        public ICollection<EventEntity> Events { get; set; } = new List<EventEntity>();
    }
}
