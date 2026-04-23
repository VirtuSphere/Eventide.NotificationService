using Eventide.NotificationService.Application.Common;
using Eventide.NotificationService.Application.DTOs;
using Eventide.NotificationService.Domain.Interfaces;
using MediatR;

namespace Eventide.NotificationService.Application.Queries.GetNotifications;

public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, Result<List<NotificationDto>>>
{
    private readonly INotificationRepository _repo;

    public GetNotificationsHandler(INotificationRepository repo) => _repo = repo;

    public async Task<Result<List<NotificationDto>>> Handle(GetNotificationsQuery req, CancellationToken ct)
    {
        var notifications = await _repo.GetByUserIdAsync(req.UserId, req.UnreadOnly, req.Skip, req.Take, ct);

        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Body = n.Body,
            Type = n.Type.ToString(),
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt
        }).ToList();

        return Result<List<NotificationDto>>.Success(dtos);
    }
}