using AutoMapper;
using NotificationService.Application.Models.Administrator;
using NotificationService.Application.Services.Abstractions;
using NotificationService.Domain;
using NotificationService.Domain.Repositories.Abstractions.Repositories;
using NotificationService.ValueObjects;

namespace NotificationService.Application.Services;

public sealed class AdministratorApplicationService(IAdministratorRepository repository, IMapper mapper) : IAdministratorApplicationService
{
    public async Task<IEnumerable<AdministratorModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(mapper.Map<AdministratorModel>);

    public async Task<AdministratorModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var administrator = await repository.GetByIdAsync(id, cancellationToken);
        return administrator is null ? null : mapper.Map<AdministratorModel>(administrator);
    }

    public async Task<AdministratorModel?> GetAdministratorByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var administrator = await repository.GetAdministratorByUsernameAsync(username, cancellationToken);
        return administrator is null ? null : mapper.Map<AdministratorModel>(administrator);
    }

    public async Task<AdministratorModel?> CreateModelAsync(CreateAdministratorModel information, CancellationToken cancellationToken = default)
    {
        if (await repository.GetAdministratorByUsernameAsync(information.Username, cancellationToken) is not null)
            return null;

        var administrator = new Administrator(Guid.NewGuid(), new Username(information.Username));
        await repository.AddAsync(administrator, cancellationToken);
        return mapper.Map<AdministratorModel>(administrator);
    }

    public async Task<bool> UpdateModelAsync(AdministratorModel information, CancellationToken cancellationToken = default)
    {
        var administrator = await repository.GetByIdAsync(information.Id, cancellationToken);
        if (administrator is null)
            return false;

        var existing = await repository.GetAdministratorByUsernameAsync(information.Username, cancellationToken);
        if (existing is not null && existing.Id != administrator.Id)
            return false;

        administrator.ChangeUsername(new Username(information.Username));
        await repository.UpdateAsync(administrator, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var administrator = await repository.GetByIdAsync(id, cancellationToken);
        if (administrator is null)
            return false;

        await repository.DeleteAsync(administrator, cancellationToken);
        return true;
    }
}