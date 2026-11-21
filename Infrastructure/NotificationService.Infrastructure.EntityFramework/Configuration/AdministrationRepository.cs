using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain;
using NotificationService.ValueObjects;
using NotificationService.ValueObjects.Validators;

namespace NotificationService.Infrastructure.EntityFramework.Configuration;

public class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
{
    public void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        
        builder.Property(x => x.Username)
            .IsRequired()
            .HasConversion(username => username.Value, str => new Username(str))
            .HasMaxLength(UsernameValidator.MAX_LENGTH);

        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.Ignore(x => x.Notifications);

        builder.HasMany<Notification>("_notifications")
            .WithOne(x => x.Administrator)
            .HasForeignKey("AdministratorId")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
