namespace NotificationService.Domain.Exceptions;

public class NotificationNotFoundException(Guid notificationId)
    : KeyNotFoundException($"Notification with ID '{notificationId}' was not found.")
{
}
