using Market.Domain.Common;
using Market.Domain.Entities.Geographical;
using Market.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Events
{
    /// <summary>
    /// Represents event organizers
    /// </summary>
    public class OrganizerEntity : BaseEntity
    {
        /// <summary>
        /// Event organizer name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Organizer description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Organizer address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Organizer city identifier
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// Organizer city
        /// </summary>
        public CityEntity City { get; set; }
        /// <summary>
        /// Organizer logo
        /// </summary>
        public byte[] Logo { get; set; }
        /// <summary>
        /// Identifier of the person that will be used for logging in 
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Person that will be a user of the organizators account
        /// </summary>
        public PersonEntity User { get; set; }

        public ICollection<EventEntity> Events { get; set; } = new List<EventEntity>();
        public ICollection<EventNewsEntity> EventNews { get; set; } = new List<EventNewsEntity>();
    }
}
