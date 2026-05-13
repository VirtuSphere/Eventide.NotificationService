using NotificationService.Domain.Base;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Exceptions;
using NotificationService.ValueObjects;

namespace NotificationService.Domain;

public class User(Guid id, Username username) : Entity<Guid>(id)
{
    public Username Username { get; private set; } = username ?? throw new ArgumentNullValueException(nameof(username));

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
    public bool MarkAsRead(Notification notification)
    {
        if (notification == null) throw new ArgumentNullValueException(nameof(notification));
        if (notification.IsRead) return false;

        notification.MarkAsRead(DateTime.UtcNow);
        return true;
    }
}
