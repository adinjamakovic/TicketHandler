using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Geographical
{
    /// <summary>
    /// Used to represent images of a certain location.
    /// </summary>
    // Programmers Note: The reason why this is separated is because a single location can have multiple pictures!
    public class LocationImagesEntity : BaseEntity
    {
        /// <summary>
        /// Location identifier
        /// </summary>
        public int LocationId { get; set; }
        public LocationEntity Location { get; set; }
        /// <summary>
        /// Location Image
        /// </summary>
        public byte[] Image { get; set; }
       
    }
}
