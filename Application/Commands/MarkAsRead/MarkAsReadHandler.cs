using Eventide.NotificationService.Application.Common;
using Eventide.NotificationService.Domain.Interfaces;
using MediatR;

namespace Eventide.NotificationService.Application.Commands.MarkAsRead;

public class MarkAsReadHandler : IRequestHandler<MarkAsReadCommand, Result>
{
    private readonly INotificationRepository _repo;

    public MarkAsReadHandler(INotificationRepository repo) => _repo = repo;

    public async Task<Result> Handle(MarkAsReadCommand req, CancellationToken ct)
    {
        var notification = await _repo.GetByIdAsync(req.NotificationId, ct);
        if (notification is null) return Result.Failure("Notification not found");

        notification.MarkAsRead();
        await _repo.SaveChangesAsync(ct);

        return Result.Success();
    }
}