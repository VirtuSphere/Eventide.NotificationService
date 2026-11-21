using NotificationService.Domain.Repositories.Abstractions.Base;

namespace NotificationService.Domain.Repositories.Abstractions.Repositories;

public interface INotificationRepository : IRepository<Notification, Guid>
{
    Task<IEnumerable<Notification>> GetByUserAsync(Guid userId, bool onlyUnread, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByAdministratorAsync(Guid administratorId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
}
