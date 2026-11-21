using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Models.Administrator;
using NotificationService.Application.Services.Abstractions;

namespace NotificationService.WebHost.Controllers;

[ApiController]
[Route("api/administrators")]
[Produces("application/json")]
public sealed class AdministratorsController(IAdministratorApplicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdministratorModel>>> GetAll(CancellationToken cancellationToken)
        => Ok(await service.GetModelsAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdministratorModel>> GetById(Guid id, CancellationToken cancellationToken)
        => await service.GetModelByIdAsync(id, cancellationToken) is { } administrator
            ? Ok(administrator)
            : NotFound();

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdministratorModel>> Create(CreateAdministratorModel model, CancellationToken cancellationToken)
        => await service.CreateModelAsync(model, cancellationToken) is { } administrator
            ? CreatedAtAction(nameof(GetById), new { id = administrator.Id }, administrator)
            : Conflict("An administrator with this username already exists.");

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, AdministratorModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest("The route and payload identifiers do not match.");
        return await service.UpdateModelAsync(model, cancellationToken)
            ? NoContent()
            : Conflict("The administrator was not found or the username is already used.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await service.DeleteModelAsync(id, cancellationToken) ? NoContent() : NotFound();
}
