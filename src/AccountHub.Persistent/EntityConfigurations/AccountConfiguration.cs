using AccountHub.Domain.Entities;
using AccountHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountHub.Persistent.EntityConfigurations
{
    internal class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.Property(x => x.Username).HasMaxLength(35);
            builder.Property(x => x.Email).HasMaxLength(254);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.Role).HasConversion(
                v => v.ToString(),
                v => (Role)Enum.Parse(typeof(Role), v));
        }
    }
}
