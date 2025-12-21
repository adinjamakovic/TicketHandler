using Market.Domain.Entities.Geographical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Geographical
{
    public sealed class LocationImagesEntityConfiguration : IEntityTypeConfiguration<LocationImagesEntity>
    {
        public void Configure(EntityTypeBuilder<LocationImagesEntity> b)
        {
            b.ToTable("Location Images");

            b.HasKey(x => x.Id);

            b.Property(x => x.Image)
                .IsRequired();

            b.HasOne(x => x.Location)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.LocationId)
                .IsRequired();
        }
    }
}
