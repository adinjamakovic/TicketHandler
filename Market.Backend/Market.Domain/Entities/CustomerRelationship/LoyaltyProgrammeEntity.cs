using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.CustomerRelationship
{
    /// <summary>
    /// Represents loyalty programmes
    /// </summary>
    public class LoyaltyProgrammeEntity : BaseEntity
    {
        /// <summary>
        /// Loyalty Programme Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Loyalty programme lower value
        /// </summary>
        public decimal MinRange {  get; set; }
        /// <summary>
        /// Loyalty programme upper value
        /// </summary>
        public decimal MaxRange { get; set; }
        /// <summary>
        /// Loyalty programme discount
        /// </summary>
        public decimal Discount { get; set; }
    }
}
