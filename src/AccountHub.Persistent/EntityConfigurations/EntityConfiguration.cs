using AccountHub.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountHub.Persistent.EntityConfigurations
{
    internal class EntityConfiguration : IEntityTypeConfiguration<Entity>
    {
        public void Configure(EntityTypeBuilder<Entity> builder)
        {
            builder.Property(x => x.Id).HasColumnOrder(0);
            builder.Property(x => x.CreatedAt).HasColumnOrder(1);
            builder.Property(x => x.UpdatedAt).HasColumnOrder(2);
            builder.UseTpcMappingStrategy();
        }
    }
}
