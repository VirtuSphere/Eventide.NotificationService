using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain;
using NotificationService.ValueObjects;
using NotificationService.ValueObjects.Validators;

namespace NotificationService.Infrastructure.EntityFramework.Configuration;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasConversion(title => title.Value, str => new Title(str))
            .HasMaxLength(TitleValidator.MAX_LENGTH);

        builder.Property(x => x.Body)
            .IsRequired()
            .HasConversion(body => body.Value, str => new Body(str))
            .HasMaxLength(BodyValidator.MAX_LENGTH);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.ReadAt)
            .IsRequired(false)
            .HasConversion(
                src => !src.HasValue
                    ? src
                    : src.Value.Kind == DateTimeKind.Utc
                        ? src
                        : DateTime.SpecifyKind(src.Value, DateTimeKind.Utc),
                dst => !dst.HasValue
                    ? dst
                    : dst.Value.Kind == DateTimeKind.Utc
                        ? dst
                        : DateTime.SpecifyKind(dst.Value, DateTimeKind.Utc));

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc));

        builder.Property(x => x.RelatedEntityType)
            .IsRequired(false)
            .HasConversion(relatedType => relatedType != null ? relatedType.Value : null,
                str => !string.IsNullOrEmpty(str) ? new RelatedEntityTypeName(str) : null);

        builder.Property<Guid?>("AdministratorId");
        builder.Property<Guid>("RecipientUserId");
        builder.Property<Guid?>("RelatedEntityId");

        builder.HasOne(x => x.RecipientUser)
            .WithMany()
            .HasForeignKey("RecipientUserId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Administrator)
            .WithMany("_notifications")
            .HasForeignKey("AdministratorId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.RelatedEntity)
            .WithMany()
            .HasForeignKey("RelatedEntityId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.IsRead);
        builder.HasIndex("RecipientUserId");
        builder.HasIndex("AdministratorId");
    }
}
