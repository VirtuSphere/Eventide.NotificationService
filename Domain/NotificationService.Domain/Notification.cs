using NotificationService.Domain.Base;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Exceptions;
using NotificationService.ValueObjects;
namespace NotificationService.Domain;

public class Notification : Entity<Guid>
{
    public User Administrator { get; private set; }
    public User RecipientUser { get; private set; }
    public Title Title { get; private set; } = null!;
    public Body Body { get; private set; } = null!;
    public NotificationType Type { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public RelatedEntityTypeName? RelatedEntityType { get; private set; }
    public Guid? RelatedEntityId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Notification()
    {
    }
    public Notification(User recipientUser, Title title, Body body, NotificationType type, DateTime createdAt, RelatedEntityTypeName? relatedEntityType = null, Guid? relatedEntityId = null) : base(Guid.NewGuid())
    {
        RecipientUser = recipientUser;
        Title = title;
        Body = body;
        Type = type;
        IsRead = false;
        ReadAt = null;
        RelatedEntityType = relatedEntityType;
        RelatedEntityId = relatedEntityId;
        CreatedAt = createdAt;
    }

    protected Notification(
        Guid id,
        User recipientUser,
        Title title,
        Body body,
        NotificationType type,
        DateTime createdAt,
        bool isRead = false,
        DateTime? readAt = null,
        RelatedEntityTypeName? relatedEntityType = null,
        Guid? relatedEntityId = null) : base(id)
    {
        RecipientUser = recipientUser ?? throw new ArgumentNullValueException(nameof(recipientUser));
        Title = title ?? throw new ArgumentNullValueException(nameof(title));
        Body = body ?? throw new ArgumentNullValueException(nameof(body));
        Type = type;
        IsRead = isRead ;
        ReadAt = readAt ?? throw new ArgumentNullValueException(nameof(readAt));
        RelatedEntityType = relatedEntityType ?? throw new ArgumentNullValueException(nameof(relatedEntityType));
        RelatedEntityId = relatedEntityId ?? throw new ArgumentNullValueException(nameof(relatedEntityId));
        CreatedAt = createdAt ;
    }

    public bool Update(
        User recipientUser,
        Title title,
        Body body,
        NotificationType type,
        RelatedEntityTypeName? relatedEntityType = null,
        Guid? relatedEntityId = null)
    {
        if (recipientUser == null) throw new ArgumentNullValueException(nameof(recipientUser));
        if (title == null) throw new ArgumentNullValueException(nameof(title));
        if (body == null) throw new ArgumentNullValueException(nameof(body));
        if (type == null) throw new ArgumentNullValueException(nameof(type));
        if (relatedEntityType == null) throw new ArgumentNullValueException(nameof(relatedEntityType));
        if (relatedEntityId == null) throw new ArgumentNullValueException(nameof(relatedEntityId));

        if (RecipientUser == recipientUser &&
            Title == title &&
            Body == body &&
            Type == type &&
            RelatedEntityType == relatedEntityType &&
            RelatedEntityId == relatedEntityId)
        {
            return false;
        }

        RecipientUser = recipientUser;
        Title = title;
        Body = body;
        Type = type;
        RelatedEntityType = relatedEntityType;
        RelatedEntityId = relatedEntityId;

        return true;
    }

    public bool MarkAsRead(DateTime readAt)
    {
        if (IsRead)
        {
            return false;
        }

        IsRead = true;
        ReadAt = readAt;
        return true;
    }

    public bool MarkAsUnread()
    {
        if (!IsRead)
        {
            return false;
        }

        IsRead = false;
        ReadAt = null;
        return true;
    }



    public override string ToString()
    {
        return $"Notification: Title={Title}, Type={Type}, IsRead={IsRead}, RecipientUser={RecipientUser}, RelatedEntityType={RelatedEntityType}, RelatedEntityId={RelatedEntityId}";
    }
}