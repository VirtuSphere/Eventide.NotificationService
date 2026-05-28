using NotificationService.Domain.Base;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Exceptions;
using NotificationService.ValueObjects;

namespace NotificationService.Domain;

public class User(Guid id, Username username) : Entity<Guid>(id)
{
    public Username Username { get; private set; } = username ?? throw new ArgumentNullValueException(nameof(username));


    public bool MarkAsRead(Notification notification)
    {
        if (notification is null) throw new ArgumentNullValueException(nameof(notification));
        if (notification.IsRead) return false;

        notification.MarkAsRead(DateTime.UtcNow);
        return true;
    }
    public bool MarkAsUnread(Notification notification)
    {
        if (notification is null) throw new ArgumentNullValueException(nameof(notification));
        if (!notification.IsRead) return false;

        notification.MarkAsUnread();
        return true;
    }
}
