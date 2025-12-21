using Market.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Events
{
    public sealed class OrganizerEntityConfiguration : IEntityTypeConfiguration<OrganizerEntity>
    {
        public void Configure(EntityTypeBuilder<OrganizerEntity> b)
        {
            b.ToTable("Organizers");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired();

            b.Property(x=>x.Address)
                .IsRequired();

            b.HasOne(x => x.User)
                .WithOne(x => x.Organizer)
                .HasForeignKey<OrganizerEntity>(x => x.UserId)
                .IsRequired();

            b.HasOne(x=>x.City)
                .WithMany(x=>x.Organizers)
                .HasForeignKey(x=>x.CityId)
                .IsRequired();
        }
    }
}
