using Market.Domain.Entities.Geographical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Geographical
{
    public sealed class LocationEntityConfiguration : IEntityTypeConfiguration<LocationEntity>
    {
        public void Configure(EntityTypeBuilder<LocationEntity> b)
        {
            b.ToTable("Locations");

            b.HasKey("Id");
        
            b.Property(x=>x.Address)
                .IsRequired();
            
            b.Property(x=>x.Name)
                .IsRequired();
            
            b.HasOne(x => x.City)
                .WithMany(x => x.Locations)
                .HasForeignKey(x => x.CityId)
                .IsRequired();
        }
    }
}
