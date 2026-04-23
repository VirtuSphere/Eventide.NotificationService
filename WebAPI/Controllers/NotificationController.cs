using Eventide.NotificationService.Application.Commands.MarkAsRead;
using Eventide.NotificationService.Application.Commands.SendNotification;
using Eventide.NotificationService.Application.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eventide.NotificationService.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendNotificationCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetNotifications(Guid userId, [FromQuery] bool unreadOnly = false)
    {
        var result = await _mediator.Send(new GetNotificationsQuery { UserId = userId, UnreadOnly = unreadOnly });
        return Ok(result.Value);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _mediator.Send(new MarkAsReadCommand { NotificationId = id });
        return result.IsSuccess ? Ok() : BadRequest(result.ErrorMessage);
    }
}