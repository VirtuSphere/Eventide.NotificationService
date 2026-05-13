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

    /// <summary> 
    /// Changes the user's username. 
    /// </summary>
    /// <param name="newUsername">New user's username.</param>
    internal bool ChangeUsername(Username newUsername)
    {
        if (newUsername == null) throw new ArgumentNullValueException(nameof(newUsername));

        if (Username == newUsername) return false;

        Username = newUsername;
        return true;
    }
    public Notification CreateNotification(User recipientUser, Title title, Body body, NotificationType type, RelatedEntityTypeName? relatedEntityType = null, Guid? relatedEntityId = null)
    {
        var notification = new Notification(recipientUser, title, body, type, DateTime.UtcNow, relatedEntityType, relatedEntityId);
        _notifications.Add(notification);
        return notification;
    }
    public bool DeleteNotification(Notification notification)
    {
        if (notification == null) throw new ArgumentNullValueException(nameof(notification));

        if (!_notifications.Contains(notification)) return false;

        _notifications.Remove(notification);
        return true;
    }

}
