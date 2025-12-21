using Market.Domain.Entities.Geographical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Geographical
{
    public sealed class CityEntityConfiguration : IEntityTypeConfiguration<CityEntity>
    {
        public void Configure(EntityTypeBuilder<CityEntity> b)
        {
            b.ToTable("Cities");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired();
        }
    }
}
