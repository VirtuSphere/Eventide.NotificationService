namespace NotificationService.Domain.Exceptions;

public class NotificationCreatedAtInvalidException(DateTime createdAt)
    : ArgumentOutOfRangeException(nameof(createdAt), createdAt, "Notification createdAt must be a valid date.");