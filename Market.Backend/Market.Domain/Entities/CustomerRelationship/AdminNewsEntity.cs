using Market.Domain.Common;
using Market.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.CustomerRelationship
{
    /// <summary>
    /// Represents a post that the administrator makes
    /// </summary>
    public class AdminNewsEntity : BaseEntity
    {
        /// <summary>
        /// Identifier of the administrator account
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// The administrator
        /// </summary>
        public PersonEntity Person { get; set; }
        /// <summary>
        /// Header text
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// Body text
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// The image
        /// </summary>
        public byte[] Image { get; set; }
    }
}
