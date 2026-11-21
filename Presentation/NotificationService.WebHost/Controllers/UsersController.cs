using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Models.User;
using NotificationService.Application.Services.Abstractions;

namespace NotificationService.WebHost.Controllers;

[ApiController]
[Route("api/users")]
[Produces("application/json")]
public sealed class UsersController(IUserApplicationService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IEnumerable<UserModel>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetAll(CancellationToken cancellationToken)
        => Ok(await service.GetModelsAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<UserModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserModel>> GetById(Guid id, CancellationToken cancellationToken)
        => await service.GetModelByIdAsync(id, cancellationToken) is { } user
            ? Ok(user)
            : NotFound();

    [HttpPost]
    [ProducesResponseType<UserModel>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserModel>> Create(CreateUserModel model, CancellationToken cancellationToken)
        => await service.CreateModelAsync(model, cancellationToken) is { } user
            ? CreatedAtAction(nameof(GetById), new { id = user.Id }, user)
            : Conflict("A user with this username already exists.");

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UserModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest("The route and payload identifiers do not match.");
        return await service.UpdateModelAsync(model, cancellationToken)
            ? NoContent()
            : Conflict("The user was not found or the username is already used.");
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await service.DeleteModelAsync(id, cancellationToken) ? NoContent() : NotFound();
}
