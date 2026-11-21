using AutoMapper;
using NotificationService.Application.Models.RelatedEntity;
using NotificationService.Application.Services.Abstractions;
using NotificationService.Domain;
using NotificationService.Domain.Repositories.Abstractions.Repositories;
using NotificationService.ValueObjects;

namespace NotificationService.Application.Services;

public sealed class RelatedEntityApplicationService(IRelatedEntityRepository repository, IMapper mapper) : IRelatedEntityApplicationService
{
    public async Task<IEnumerable<RelatedEntityModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(mapper.Map<RelatedEntityModel>);

    public async Task<RelatedEntityModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : mapper.Map<RelatedEntityModel>(entity);
    }

    public async Task<RelatedEntityModel?> CreateModelAsync(CreateRelatedEntityModel information, CancellationToken cancellationToken = default)
    {
        var entity = new RelatedEntity(
            Guid.NewGuid(),
            string.IsNullOrWhiteSpace(information.DisplayName) ? null : new RelatedEntityDisplayName(information.DisplayName),
            string.IsNullOrWhiteSpace(information.ExternalUrl) ? null : new ExternalUrl(information.ExternalUrl));
        await repository.AddAsync(entity, cancellationToken);
        return mapper.Map<RelatedEntityModel>(entity);
    }

    public async Task<bool> UpdateModelAsync(RelatedEntityModel information, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(information.Id, cancellationToken);
        if (entity is null)
            return false;

        entity.UpdateDisplay(
            string.IsNullOrWhiteSpace(information.DisplayName) ? null : new RelatedEntityDisplayName(information.DisplayName),
            string.IsNullOrWhiteSpace(information.ExternalUrl) ? null : new ExternalUrl(information.ExternalUrl));
        await repository.UpdateAsync(entity, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return false;

        await repository.DeleteAsync(entity, cancellationToken);
        return true;
    }
}
