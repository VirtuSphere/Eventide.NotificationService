namespace NotificationService.Domain.Exceptions;

public class NotificationTitleNullException()
    : ArgumentNullException("title", "Notification title must be specified.");