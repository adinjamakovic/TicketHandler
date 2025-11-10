using Market.Domain.Common;
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
    }
}
