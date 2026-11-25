using NotificationService.Application.Models.Notification;

namespace NotificationService.Application.Services.Abstractions;

public interface INotificationApplicationService
{
    Task<IEnumerable<NotificationModel>> GetModelsAsync(Guid? userId = null, Guid? administratorId = null, bool onlyUnread = false, CancellationToken cancellationToken = default);
    Task<NotificationModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NotificationModel?> CreateModelAsync(CreateNotificationModel information, CancellationToken cancellationToken = default);
    Task<bool> UpdateModelAsync(Guid id, CreateNotificationModel information, CancellationToken cancellationToken = default);
    Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> MarkAsUnreadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
}
