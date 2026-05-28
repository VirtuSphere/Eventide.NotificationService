using NotificationService.Domain.Base;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Exceptions;
using NotificationService.ValueObjects;

namespace NotificationService.Domain;

public class Administrator(Guid id, Username username) : Entity<Guid>(id)
{
    public Username Username { get; private set; } = username ?? throw new ArgumentNullValueException(nameof(username));
    private readonly List<Notification> _notifications = [];
    public IReadOnlyCollection<Notification> Notifications => _notifications.ToList().AsReadOnly();


    public Notification CreateNotification(User recipientUser, Title title, Body body, NotificationType type, RelatedEntityTypeName? relatedEntityType = null, RelatedEntity? relatedEntity = null)
    {
        var notification = new Notification(recipientUser, title, body, type, DateTime.UtcNow, this, relatedEntityType, relatedEntity);
        if (_notifications.Contains(notification)) throw new NotificationNotFoundException(notification.Id);
        _notifications.Add(notification);
        return notification;
    }
    public bool DeleteNotification(Notification notification)
    {
        if (notification is null) throw new ArgumentNullValueException(nameof(notification));

        if (!_notifications.Contains(notification)) return false;

        _notifications.Remove(notification);
        return true;
    }
    public bool UpdateNotification(Notification notification, Title? title = null, Body? body = null, NotificationType? type = null, RelatedEntityTypeName? relatedEntityType = null, RelatedEntity? relatedEntity = null)
    {
        if (notification is null) throw new ArgumentNullValueException(nameof(notification));

        if (!_notifications.Contains(notification)) return false;

        bool isUpdated = title != null || body != null || type != null || relatedEntityType != null || relatedEntity != null;
        return isUpdated;
    }

}
