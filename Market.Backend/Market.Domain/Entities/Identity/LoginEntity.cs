using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Identity
{
    /// <summary>
    /// Represents table with login information of a certain person
    /// </summary>
    public class LoginEntity : BaseEntity
    {
        /// <summary>
        /// User username
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Password salt
        /// </summary>
        public string Salt { get; set; }
        /// <summary>
        /// Password hash
        /// </summary>
        public string PasswordHash {  get; set; }
    }
}
