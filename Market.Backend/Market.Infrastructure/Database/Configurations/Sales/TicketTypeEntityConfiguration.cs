using Market.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Sales
{
    public sealed class TicketTypeEntityConfiguration : IEntityTypeConfiguration<TicketTypeEntity>
    {
        public void Configure(EntityTypeBuilder<TicketTypeEntity> b)
        {
            b.ToTable("Ticket types");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .IsRequired();
        }
    }
}
