using Market.Domain.Entities.Geographical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Geographical
{
    public sealed class VenueEntityConfiguration : IEntityTypeConfiguration<VenueEntity>
    {
        public void Configure(EntityTypeBuilder<VenueEntity> b)
        {
            b.ToTable("Venues");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired();

            b.HasOne(x => x.Location)
                .WithMany(x => x.Venues)
                .HasForeignKey(x => x.LocationId)
                .IsRequired();
        }
    }
}
