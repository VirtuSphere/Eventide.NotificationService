using NotificationService.Domain.Repositories.Abstractions.Base;
using NotificationService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Repositories.Abstractions.Repositories;

public interface IAdministratorRepository : IRepository<Administrator, Guid>
{
    // Так как имя пользователя уникальное
    Task<Administrator?> GetAdministratorByUsernameAsync(string username, CancellationToken cancellationToken);
}