using Market.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Sales
{
    public sealed class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItemEntity>
    {
        public void Configure(EntityTypeBuilder<CartItemEntity> b)
        {
            b.ToTable("Cart Items");

            b.HasKey(x => new { x.PersonId, x.TicketId});

            b.Property(x => x.Quantity)
                .IsRequired();

            b.HasOne(x => x.Ticket)
                .WithMany()
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Person)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.PersonId)
                .IsRequired();
        }
    }
}
