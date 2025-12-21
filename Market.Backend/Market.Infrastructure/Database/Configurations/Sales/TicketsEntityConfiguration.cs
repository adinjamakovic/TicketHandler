using Market.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Sales
{
    public sealed class TicketsEntityConfiguration : IEntityTypeConfiguration<TicketsEntity>
    {
        public void Configure(EntityTypeBuilder<TicketsEntity> b)
        {
            b.ToTable("Tickets");

            b.HasKey(x => x.Id);

            b.Property(x=>x.UnitPrice)
                .IsRequired();

            b.Property(x=>x.QuanityInStock)
                .IsRequired();

            b.HasOne(x => x.Event)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.EventId)
                .IsRequired();

            b.HasOne(x=>x.TicketType)
                .WithMany(x=>x.Tickets)
                .HasForeignKey(x=>x.TicketTypeId)
                .IsRequired();
        }
    }
}
