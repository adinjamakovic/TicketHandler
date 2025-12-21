using Market.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Sales
{
    public sealed class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> b)
        {
            b.ToTable("Orders");
            b.HasKey(x=> x.Id);
            b.HasOne(x => x.Person)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.PersonId)
                .IsRequired();
        }
    }
}
