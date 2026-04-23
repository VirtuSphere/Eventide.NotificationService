using Eventide.NotificationService.Domain.Enums;
using Eventide.NotificationService.Domain.Exceptions;

namespace Eventide.NotificationService.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid RecipientUserId { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public NotificationType Type { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public Guid? RelatedEntityId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Notification() { }

    public static Notification Create(
        Guid recipientUserId, string title, string body, NotificationType type,
        string? relatedEntityType = null, Guid? relatedEntityId = null)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(body)) throw new DomainException("Body cannot be empty");

        return new Notification
        {
            Id = Guid.NewGuid(),
            RecipientUserId = recipientUserId,
            Title = title,
            Body = body,
            Type = type,
            IsRead = false,
            RelatedEntityType = relatedEntityType,
            RelatedEntityId = relatedEntityId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}