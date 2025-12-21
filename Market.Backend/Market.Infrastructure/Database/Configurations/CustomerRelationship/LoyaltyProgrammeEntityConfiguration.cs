using Market.Domain.Entities.CustomerRelationship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.CustomerRelationship
{
    public sealed class LoyaltyProgrammeEntityConfiguration : IEntityTypeConfiguration<LoyaltyProgrammeEntity>
    {
        public void Configure(EntityTypeBuilder<LoyaltyProgrammeEntity> b)
        {
            b.ToTable("Loyalty Programmes");
            b.HasKey(x=>x.Id);
            b.Property(x => x.Name)
                .IsRequired();
            b.Property(x => x.Name)
                .HasDefaultValue(0);
            b.Property(x => x.Discount)
                .IsRequired();
        }
    }
}
