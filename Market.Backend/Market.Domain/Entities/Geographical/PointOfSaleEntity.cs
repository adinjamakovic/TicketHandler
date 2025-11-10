using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Geographical
{
    /// <summary>
    /// Represents the table which contains information about points of sale for tickets
    /// </summary>
    public class PointOfSaleEntity : BaseEntity
    {
        /// <summary>
        /// Point of sale name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Point of sale Address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Point of sale city identifier
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// Point of sale city
        /// </summary>
        public CityEntity City { get; set; }
        /// <summary>
        /// Point of sale picture
        /// </summary>
        public byte[] Picture { get; set; }
    }
}
