using Market.Domain.Common;
using Market.Domain.Entities.Events;
using Market.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Geographical
{
    /// <summary>
    /// Represents the table which will contain city information
    /// </summary>
    public class CityEntity : BaseEntity
    {
        /// <summary>
        /// Identifier of the country in which the city is located
        /// </summary>
        public int CountryId { get; set; }
        /// <summary>
        /// Country in which the city is located
        /// </summary>
        public CountryEntity Country { get; set; }
        /// <summary>
        /// City Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// City Postal Code
        /// </summary>
        public string PostalCode { get; set; }

        public ICollection<PointOfSaleEntity> PointsOfSale { get; private set; } = new List<PointOfSaleEntity>();
        public ICollection<PersonEntity> Persons { get; private set; } = new List<PersonEntity>();
        public ICollection<LocationEntity> Locations { get; private set; } = new List<LocationEntity>();
        public ICollection<OrganizerEntity> Organizers { get; private set;} = new List<OrganizerEntity>();
    }
}
