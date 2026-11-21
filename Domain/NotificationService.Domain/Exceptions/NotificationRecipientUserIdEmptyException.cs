namespace NotificationService.Domain.Exceptions;

public class NotificationRecipientUserIdEmptyException()
    : ArgumentException("Recipient user id must not be empty.", "recipientUserId");