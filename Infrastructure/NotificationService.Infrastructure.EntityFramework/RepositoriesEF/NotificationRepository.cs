using NotificationService.Domain;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Infrastructure.EntityFramework.RepositoriesEF;

public class EfNotificationRepository(ApplicationDbContext context)
    : EfRepository<Notification, Guid>(context)
{
    private readonly DbSet<Notification> _notifications = context.Set<Notification>();

    public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken,
        bool onlyUnread = false)
    {
        var query = _notifications
            .Where(x => EF.Property<Guid>(x, "RecipientUserId") == userId);

        if (onlyUnread)
            query = query.Where(x => !x.IsRead);

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _notifications
            .Where(x => EF.Property<Guid>(x, "RecipientUserId") == userId && !x.IsRead)
            .CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByAdministratorIdAsync(
        Guid administratorId,
        CancellationToken cancellationToken)
    {
        return await _notifications
            .Where(x => EF.Property<Guid?>(x, "AdministratorId") == administratorId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
