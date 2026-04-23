using Eventide.NotificationService.Application.Common;
using MediatR;

namespace Eventide.NotificationService.Application.Commands.MarkAsRead;

public class MarkAsReadCommand : IRequest<Result>
{
    public Guid NotificationId { get; init; }
}