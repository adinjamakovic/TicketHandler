using Market.Domain.Entities.CustomerRelationship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.CustomerRelationship
{
    public sealed class TransactionEntityConfiguration : IEntityTypeConfiguration<TransactionEntity>
    {
        public void Configure(EntityTypeBuilder<TransactionEntity> b)
        {
            b.ToTable("Transactions");

            b.HasKey(x =>new { x.Id, x.OrderId});

            b.Property(x => x.Status)
                .HasConversion(
                    x => x.ToString(),
                    x => Enum.Parse<OrderStatusType>(x)
                );

            b.HasOne(x => x.Order)
                .WithOne()
                .HasForeignKey<TransactionEntity>(x => x.OrderId)
                .IsRequired();

            b.HasOne(x => x.Person)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.PersonId)
                .IsRequired();
        }
    }
}
