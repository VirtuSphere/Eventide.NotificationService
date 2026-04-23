using Eventide.NotificationService.Application.Common;
using Eventide.NotificationService.Domain.Entities;
using Eventide.NotificationService.Domain.Interfaces;
using MediatR;

namespace Eventide.NotificationService.Application.Commands.SendNotification;

public class SendNotificationHandler : IRequestHandler<SendNotificationCommand, Result<Guid>>
{
    private readonly INotificationRepository _repo;

    public SendNotificationHandler(INotificationRepository repo) => _repo = repo;

    public async Task<Result<Guid>> Handle(SendNotificationCommand req, CancellationToken ct)
    {
        var notification = Notification.Create(
            req.RecipientUserId, req.Title, req.Body, req.Type,
            req.RelatedEntityType, req.RelatedEntityId);

        await _repo.AddAsync(notification, ct);
        await _repo.SaveChangesAsync(ct);

        return Result<Guid>.Success(notification.Id);
    }
}