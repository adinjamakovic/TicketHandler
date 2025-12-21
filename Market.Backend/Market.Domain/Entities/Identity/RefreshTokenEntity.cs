using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Identity
{
    public sealed class RefreshTokenEntity : BaseEntity
    {
        public string TokenHash { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public bool IsRevoked { get; set; }
        public int UserId { get; set; }
        public PersonEntity User { get; set; }
        public string Fingerprint { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
    }
}
