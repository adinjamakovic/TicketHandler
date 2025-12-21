using Market.Domain.Entities.CustomerRelationship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.CustomerRelationship
{
    public sealed class AdminNewsEntityConfiguration : IEntityTypeConfiguration<AdminNewsEntity>
    {
        public void Configure(EntityTypeBuilder<AdminNewsEntity> b)
        {
            b.ToTable("Admin news");

            b.HasKey(x => x.Id);

            b.Property(x => x.Header)
                .IsRequired();

            b.HasOne(x => x.Person)
                .WithMany()
                .HasForeignKey(x => x.PersonId)
                .IsRequired();
        }
    }
}
