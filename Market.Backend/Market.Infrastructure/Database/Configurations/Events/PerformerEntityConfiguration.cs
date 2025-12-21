using Market.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Events
{
    public sealed class PerformerEntityConfiguration : IEntityTypeConfiguration<PerformerEntity>
    {
        public void Configure(EntityTypeBuilder<PerformerEntity> b)
        {
            b.ToTable("Performes");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired();

            b.HasOne(x => x.Genre)
                .WithMany(x => x.Performers)
                .HasForeignKey(x => x.GenreId)
                .IsRequired();
        }
    }
}
