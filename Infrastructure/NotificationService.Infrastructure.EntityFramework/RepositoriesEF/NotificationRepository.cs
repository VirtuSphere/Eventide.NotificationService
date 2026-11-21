using NotificationService.Domain;
using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Repositories.Abstractions.Repositories;

namespace NotificationService.Infrastructure.EntityFramework.RepositoriesEF;

public class EfNotificationRepository(ApplicationDbContext context)
    : EfRepository<Notification, Guid>(context), INotificationRepository
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

    public Task<IEnumerable<Notification>> GetByUserAsync(
        Guid userId,
        bool onlyUnread,
        CancellationToken cancellationToken = default)
        => GetNotificationsByUserIdAsync(userId, cancellationToken, onlyUnread);

    public Task<IEnumerable<Notification>> GetByAdministratorAsync(
        Guid administratorId,
        CancellationToken cancellationToken = default)
        => GetNotificationsByAdministratorIdAsync(administratorId, cancellationToken);

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
