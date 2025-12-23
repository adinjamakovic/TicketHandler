using Market.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Events
{
    public sealed class EventEntityConfiguration : IEntityTypeConfiguration<EventEntity>
    {
        public void Configure(EntityTypeBuilder<EventEntity> b)
        {
            b.ToTable("Events");

            b.HasKey(x=>x.Id);

            b.Property(x => x.Name)
                .IsRequired();

            b.HasOne(x => x.Organizer)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.OrganizerId)
                .IsRequired();

            b.HasOne(x => x.Venue)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x=>x.EventType)
                .WithMany(x => x.Events)
                .HasForeignKey(x=>x.EventTypeId)
                .IsRequired();
        }
    }
}
