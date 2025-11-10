using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Geographical
{
    /// <summary>
    /// Represents places where events will be performed
    /// </summary>
    public class LocationEntity : BaseEntity
    {
        /// <summary>
        /// Location Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Identifier for the city in which the location is located
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// City in which the location is located
        /// </summary>
        public CityEntity City { get; set; }
        /// <summary>
        /// Location address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Location description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// A list of location images
        /// </summary>
        public List<LocationImagesEntity> Images { get; set; }
    }
}
