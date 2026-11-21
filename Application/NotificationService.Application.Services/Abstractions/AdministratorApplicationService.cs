using NotificationService.Application.Models.Administrator;
using NotificationService.Application.Services.Abstractions.Base;

namespace NotificationService.Application.Services.Abstractions;

public interface IAdministratorApplicationService : IApplicationService<AdministratorModel, CreateAdministratorModel, Guid>
{
    Task<AdministratorModel?> GetAdministratorByUsernameAsync(string username, CancellationToken cancellationToken = default);
}