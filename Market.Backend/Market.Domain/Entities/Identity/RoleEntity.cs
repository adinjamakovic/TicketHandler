using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Identity
{
    /// <summary>
    /// Represents the Users role
    /// </summary>
    public class RoleEntity : BaseEntity
    {
        /// <summary>
        /// Role name
        /// </summary>
       public string Name { get; set; }
    }
}
