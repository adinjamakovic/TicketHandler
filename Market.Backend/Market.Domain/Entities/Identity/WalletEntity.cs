using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Identity
{
    /// <summary>
    /// Represents the users current wallet balance
    /// </summary>
    public class WalletEntity : BaseEntity
    {
        /// <summary>
        /// Represents the current amount of funds
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// Reoresents the amount of funds the user has spent 
        /// </summary>
        public decimal PreviouslySpent { get; set; }
    }
}
