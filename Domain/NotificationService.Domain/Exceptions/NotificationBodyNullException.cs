namespace NotificationService.Domain.Exceptions;

public class NotificationBodyNullException()
    : ArgumentNullException("body", "Notification body must be specified.");