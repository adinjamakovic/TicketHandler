using Market.Domain.Common;
using Market.Domain.Entities.Geographical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Identity
{
    /// <summary>
    /// Represents the table which will containt personal user information
    /// </summary>
    public class PersonEntity : BaseEntity
    {
        /// <summary>
        /// Person First Name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Person Last Name
        /// </summary>
        public string LastName { get; set; } 
        /// <summary>
        /// Person birth date
        /// </summary>
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// Identifier of the City in which the person resides
        /// </summary>
        public int CityId  { get; set; }
        /// <summary>
        /// City in which the person resides
        /// </summary>
        public CityEntity City { get; set; }
        /// <summary>
        /// Address at which the person resides
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// The gender of the person
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// Contact phone number
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Identifier of the role that the person is a part of
        /// </summary>
        public int RoleId { get; set; }
    }
}
