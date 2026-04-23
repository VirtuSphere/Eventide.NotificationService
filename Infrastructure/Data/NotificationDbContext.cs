using Eventide.NotificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eventide.NotificationService.Infrastructure.Data;

public class NotificationDbContext : DbContext
{
    public DbSet<Notification> Notifications => Set<Notification>();

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(builder =>
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
            builder.Property(n => n.Body).IsRequired().HasMaxLength(2000);
            builder.Property(n => n.Type).HasConversion<string>().IsRequired();
            builder.HasIndex(n => n.RecipientUserId);
            builder.HasIndex(n => n.IsRead);
        });
    }
}