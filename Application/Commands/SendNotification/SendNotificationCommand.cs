using Eventide.NotificationService.Application.Common;
using Eventide.NotificationService.Domain.Enums;
using MediatR;

namespace Eventide.NotificationService.Application.Commands.SendNotification;

public class SendNotificationCommand : IRequest<Result<Guid>>
{
    public Guid RecipientUserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public NotificationType Type { get; init; }
    public string? RelatedEntityType { get; init; }
    public Guid? RelatedEntityId { get; init; }
}