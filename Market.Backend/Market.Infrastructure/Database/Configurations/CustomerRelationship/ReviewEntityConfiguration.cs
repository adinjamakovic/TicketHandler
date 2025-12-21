using Market.Domain.Entities.CustomerRelationship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.CustomerRelationship
{
    public sealed class ReviewEntityConfiguration : IEntityTypeConfiguration<ReviewEntity>
    {
        public void Configure(EntityTypeBuilder<ReviewEntity> b)
        {
            b.ToTable("Reviews");

            b.HasKey(x => new {x.PersonId, x.EventId});

            b.HasOne(x => x.Person)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.PersonId)
                .IsRequired();

            b.HasOne(x => x.Event)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.EventId)
                .IsRequired();

        }
    }
}
