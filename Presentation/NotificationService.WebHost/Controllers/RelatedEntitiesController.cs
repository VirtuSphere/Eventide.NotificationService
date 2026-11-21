using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Models.RelatedEntity;
using NotificationService.Application.Services.Abstractions;

namespace NotificationService.WebHost.Controllers;

[ApiController]
[Route("api/related-entities")]
[Produces("application/json")]
public sealed class RelatedEntitiesController(IRelatedEntityApplicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RelatedEntityModel>>> GetAll(CancellationToken cancellationToken)
        => Ok(await service.GetModelsAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RelatedEntityModel>> GetById(Guid id, CancellationToken cancellationToken)
        => await service.GetModelByIdAsync(id, cancellationToken) is { } entity
            ? Ok(entity)
            : NotFound();

    [HttpPost]
    public async Task<ActionResult<RelatedEntityModel>> Create(CreateRelatedEntityModel model, CancellationToken cancellationToken)
    {
        var entity = await service.CreateModelAsync(model, cancellationToken);
        return entity is null
            ? BadRequest()
            : CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, RelatedEntityModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest("The route and payload identifiers do not match.");
        return await service.UpdateModelAsync(model, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await service.DeleteModelAsync(id, cancellationToken) ? NoContent() : NotFound();
}
