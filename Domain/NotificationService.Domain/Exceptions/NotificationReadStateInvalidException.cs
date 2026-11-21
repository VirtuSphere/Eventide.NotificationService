namespace NotificationService.Domain.Exceptions;

public class NotificationReadStateInvalidException()
    : InvalidOperationException("Read state is invalid: IsRead and ReadAt values are inconsistent.");