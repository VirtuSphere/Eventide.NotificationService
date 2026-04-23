using Eventide.NotificationService.Application.Common;
using Eventide.NotificationService.Application.DTOs;
using MediatR;

namespace Eventide.NotificationService.Application.Queries.GetNotifications;

public class GetNotificationsQuery : IRequest<Result<List<NotificationDto>>>
{
    public Guid UserId { get; init; }
    public bool UnreadOnly { get; init; }
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 20;
}