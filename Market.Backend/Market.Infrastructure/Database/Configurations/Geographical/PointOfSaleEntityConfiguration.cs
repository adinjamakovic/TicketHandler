using Market.Domain.Entities.Geographical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Geographical
{
    public sealed class PointOfSaleEntityConfiguration : IEntityTypeConfiguration<PointOfSaleEntity>
    {
        public void Configure(EntityTypeBuilder<PointOfSaleEntity> b)
        {
            b.ToTable("PointsOfSale");

            b.HasKey(x => x.Id);

            b.HasOne(x => x.City)
                .WithMany(x=>x.PointsOfSale)
                .HasForeignKey(x=>x.CityId)
                .IsRequired();
        }
    }
}
