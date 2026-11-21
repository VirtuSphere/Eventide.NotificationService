using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Models.Notification;
using NotificationService.Application.Services.Abstractions;

namespace NotificationService.WebHost.Controllers;

[ApiController]
[Route("api/notifications")]
[Produces("application/json")]
public sealed class NotificationsController(INotificationApplicationService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IEnumerable<NotificationModel>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationModel>>> GetAll(
        [FromQuery] Guid? userId,
        [FromQuery] Guid? administratorId,
        [FromQuery] bool onlyUnread,
        CancellationToken cancellationToken)
        => Ok(await service.GetModelsAsync(userId, administratorId, onlyUnread, cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationModel>> GetById(Guid id, CancellationToken cancellationToken)
        => await service.GetModelByIdAsync(id, cancellationToken) is { } notification
            ? Ok(notification)
            : NotFound();

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount([FromQuery] Guid userId, CancellationToken cancellationToken)
        => Ok(await service.GetUnreadCountAsync(userId, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NotificationModel>> Create(CreateNotificationModel model, CancellationToken cancellationToken)
        => await service.CreateModelAsync(model, cancellationToken) is { } notification
            ? CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification)
            : BadRequest("Recipient, administrator, or related entity was not found.");

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, CreateNotificationModel model, CancellationToken cancellationToken)
        => await service.UpdateModelAsync(id, model, cancellationToken)
            ? NoContent()
            : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await service.DeleteModelAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
        => await service.MarkAsReadAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPatch("{id:guid}/unread")]
    public async Task<IActionResult> MarkAsUnread(Guid id, CancellationToken cancellationToken)
        => await service.MarkAsUnreadAsync(id, cancellationToken) ? NoContent() : NotFound();
}
