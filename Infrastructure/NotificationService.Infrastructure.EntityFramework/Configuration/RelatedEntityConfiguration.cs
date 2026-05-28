using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain;
using NotificationService.ValueObjects;
using NotificationService.ValueObjects.Validators;

namespace NotificationService.Infrastructure.EntityFramework.Configuration
{
public class RelatedEntityConfiguration : IEntityTypeConfiguration<RelatedEntity>
{
    public void Configure(EntityTypeBuilder<RelatedEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.DisplayName)
            .HasConversion(
                value => value == null ? null : value.Value,
                value => value == null ? null : new RelatedEntityDisplayName(value))
            .HasMaxLength(RelatedEntityDisplayNameValidator.MAX_LENGTH);

        builder.Property(x => x.ExternalUrl)
            .HasConversion(
                value => value == null ? null : value.Value,
                value => value == null ? null : new ExternalUrl(value))
            .HasMaxLength(ExternalUrlValidator.MAX_LENGTH);
    }
}
}
