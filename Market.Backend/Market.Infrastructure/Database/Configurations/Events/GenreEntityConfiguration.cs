using Market.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Events
{
    public sealed class GenreEntityConfiguration : IEntityTypeConfiguration<GenreEntity>
    {
        public void Configure(EntityTypeBuilder<GenreEntity> b)
        {
            b.ToTable("Genres");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired();

        }
    }
}
