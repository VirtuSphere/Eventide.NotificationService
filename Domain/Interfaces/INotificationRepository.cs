using Eventide.NotificationService.Domain.Entities;

namespace Eventide.NotificationService.Domain.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserIdAsync(Guid userId, bool unreadOnly = false, int skip = 0, int take = 20, CancellationToken ct = default);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Notification notification, CancellationToken ct = default);
    Task UpdateAsync(Notification notification, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}