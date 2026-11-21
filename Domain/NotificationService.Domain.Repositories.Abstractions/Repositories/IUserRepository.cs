using NotificationService.Domain.Repositories.Abstractions.Base;
using NotificationService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Repositories.Abstractions.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    // Так как имя пользователя уникальное
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
}