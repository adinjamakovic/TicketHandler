using Market.Domain.Common;
using Market.Domain.Entities.CustomerRelationship;
using Market.Domain.Entities.Events;
using Market.Domain.Entities.Geographical;
using Market.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Market.Domain.Entities.Identity
{
    /// <summary>
    /// Represents the table which will containt personal user information
    /// </summary>
    public class PersonEntity : BaseEntity
    {
        public WalletEntity Wallet { get; set; }
        /// <summary>
        /// Person First Name
        /// </summary>
        public string? FirstName { get; set; }
        /// <summary>
        /// Person Last Name
        /// </summary>
        public string? LastName { get; set; } 
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
        public string? Address { get; set; }
        /// <summary>
        /// The gender of the person
        /// </summary>
        public string? Gender { get; set; }
        /// <summary>
        /// Contact phone number
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// User username
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Password salt
        /// </summary>
        public string? Salt { get; set; }
        /// <summary>
        /// Password hash
        /// </summary>
        public string PasswordHash { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsOrganiser { get; set; }
        public bool IsUser { get; set; }
        public int TokenVersion { get; set; }
        public bool IsEnabled { get; set; }
        public OrganizerEntity? Organizer { get; set; }
        public ICollection<RefreshTokenEntity> RefreshTokens { get; private set; } = new List<RefreshTokenEntity>();
        public ICollection<CartItemEntity> CartItems { get; set; } = new List<CartItemEntity>();
        public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
        public ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
        public ICollection<ReviewEntity> Reviews { get; set; } = new List<ReviewEntity>();
    }
}
