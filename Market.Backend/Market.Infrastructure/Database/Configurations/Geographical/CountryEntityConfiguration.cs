using Market.Domain.Entities.Geographical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Geographical
{
    public sealed class CountryEntityConfiguration : IEntityTypeConfiguration<CountryEntity>
    {
        public void Configure(EntityTypeBuilder<CountryEntity> b)
        {
            b.ToTable("Countries");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired();

            b.HasMany(x => x.Cities)
                .WithOne(x => x.Country)
                .HasForeignKey(x => x.CountryId)
                .IsRequired();
        }
    }
}
