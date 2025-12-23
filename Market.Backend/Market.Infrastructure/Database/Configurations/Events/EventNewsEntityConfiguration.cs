using Market.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Events
{
    public sealed class EventNewsEntityConfiguration : IEntityTypeConfiguration<EventNewsEntity>
    {
        public void Configure(EntityTypeBuilder<EventNewsEntity> b)
        {
            b.ToTable("EventNews");

            b.HasKey(x => x.Id);

            b.Property(x => x.Header)
                .IsRequired();

            b.HasOne(x => x.Organizer)
                .WithMany(x => x.EventNews)
                .HasForeignKey(x => x.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Event)
                .WithMany(x => x.EventNews)
                .HasForeignKey(x => x.EventId)
                .IsRequired();
        }
    }
}
