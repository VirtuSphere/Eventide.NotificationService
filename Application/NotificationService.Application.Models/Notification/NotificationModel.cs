using NotificationService.Application.Models.Base;
using NotificationService.Domain.Enums;

namespace NotificationService.Application.Models.Notification;

public sealed record NotificationModel(
    Guid Id,
    Guid RecipientUserId,
    Guid? AdministratorId,
    string Title,
    string Body,
    NotificationType Type,
    bool IsRead,
    DateTime? ReadAt,
    string? RelatedEntityType,
    Guid? RelatedEntityId,
    DateTime CreatedAt) : IModel<Guid>;
