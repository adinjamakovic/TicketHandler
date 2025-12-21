using Market.Domain.Entities.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Events
{
    public sealed class EventTypeEntityConfiguration : IEntityTypeConfiguration<EventTypeEntity>
    {
        public void Configure(EntityTypeBuilder<EventTypeEntity> b)
        {
            b.ToTable("EventTypes");

            b.HasKey(x=> x.Id);

            b.Property(x => x.Name)
                .IsRequired();
        }
    }
}
