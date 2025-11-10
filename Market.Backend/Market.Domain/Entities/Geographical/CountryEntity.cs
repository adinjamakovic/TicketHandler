using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Geographical
{
    /// <summary>
    /// Represents the table in which countries will be stored
    /// </summary>
    public class CountryEntity : BaseEntity
    {
        /// <summary>
        /// Country name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Country flag image
        /// </summary>
        public byte[] Flag { get; set; }
        /// <summary>
        /// Distinct phone code
        /// </summary>
        public string PhoneCode { get; set; }
    }
}
