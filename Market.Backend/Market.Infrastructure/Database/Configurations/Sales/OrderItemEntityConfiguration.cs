using Market.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Sales
{
    public sealed class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItemEntity>
    {
        public void Configure(EntityTypeBuilder<OrderItemEntity> b)
        {
            b.ToTable("Order Items");

            b.HasKey(x => x.Id);

            b.Property(x => x.Quantity)
                .IsRequired();

            b.Property(x => x.DiscountPercent)
                .HasDefaultValue(0M);

            b.HasOne(x => x.Order)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.OrderId)
                .IsRequired();

            b.HasOne(x => x.Ticket)
                .WithMany()
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
