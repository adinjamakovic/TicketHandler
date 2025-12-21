using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Infrastructure.Database.Configurations.Identity
{
    public sealed class PersonEntityConfiguration : IEntityTypeConfiguration<PersonEntity>
    {
        public void Configure(EntityTypeBuilder<PersonEntity> b)
        {
            b.ToTable("Persons");
            
            b.HasKey(x => x.Id);

            b.HasIndex(x => x.Email).
                IsUnique();

            b.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.PasswordHash)
                .IsRequired();

            b.Property(x => x.IsAdmin)
                .HasDefaultValue(false);

            b.Property(x => x.IsUser)
                .HasDefaultValue(true);

            b.Property(x => x.IsOrganiser)
                .HasDefaultValue(false);

            b.Property(x => x.TokenVersion)
                .HasDefaultValue(0);

            b.Property(x => x.IsEnabled)
                .HasDefaultValue(true);

            b.HasMany(x => x.RefreshTokens)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            b.HasOne(x => x.Wallet)
                .WithOne(x => x.Person)
                .HasForeignKey<WalletEntity>(x => x.PersonId)
                .IsRequired();

            b.HasOne(x => x.City)
                .WithMany(x => x.Persons)
                .HasForeignKey(x => x.CityId)
                .IsRequired();
        }
    }
}
