using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Identity
{
    public sealed class WalletEntityConfiguration : IEntityTypeConfiguration<WalletEntity>
    {
        public void Configure(EntityTypeBuilder<WalletEntity> b)
        {
            b.ToTable("Wallets");

            b.HasKey("Id");

            b.Property(x => x.Balance)
                .HasDefaultValue(0);

            b.Property(x=>x.PreviouslySpent)
                .HasDefaultValue(0);
        }
    }
}
