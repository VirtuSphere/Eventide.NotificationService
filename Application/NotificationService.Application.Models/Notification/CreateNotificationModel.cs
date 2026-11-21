using NotificationService.Application.Models.Base;
using NotificationService.Domain.Enums;

namespace NotificationService.Application.Models.Notification;

public sealed record CreateNotificationModel(
    Guid RecipientUserId,
    Guid? AdministratorId,
    string Title,
    string Body,
    NotificationType Type,
    string? RelatedEntityType,
    Guid? RelatedEntityId) : ICreateModel;
