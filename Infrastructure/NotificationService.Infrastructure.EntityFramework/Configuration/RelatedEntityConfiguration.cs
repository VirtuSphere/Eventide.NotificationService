using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain;

namespace NotificationService.Infrastructure.EntityFramework.Configuration
{
public class RelatedEntityConfiguration : IEntityTypeConfiguration<RelatedEntity>
{
    public void Configure(EntityTypeBuilder<RelatedEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
    }
}
}
