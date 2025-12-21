using Market.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Events
{
    public sealed class PerformerEventEntityConfiguration : IEntityTypeConfiguration<PerformerEventEntity>
    {
        public void Configure(EntityTypeBuilder<PerformerEventEntity> b)
        {
            b.ToTable("Performer Events");

            b.HasKey(x => x.Id);

            b.HasOne(x => x.Performer)
                .WithMany(x => x.PerformerEvents)
                .HasForeignKey(x => x.PerformerId)
                .IsRequired();

            b.HasOne(x=>x.Event)
                .WithMany(x=>x.PerformerEvents)
                .HasForeignKey(x=>x.EventId)
                .IsRequired();
        }
    }
}
