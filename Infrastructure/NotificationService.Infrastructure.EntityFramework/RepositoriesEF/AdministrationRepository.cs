using NotificationService.Domain;
using NotificationService.Domain.Repositories.Abstractions;
using NotificationService.Domain.Repositories.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Infrastructure.EntityFramework.RepositoriesEF;

public class EfAdministratorRepository(ApplicationDbContext context)
    : EfRepository<Administrator, Guid>(context), IAdministratorRepository
{
    private readonly DbSet<Administrator> _administrators = context.Set<Administrator>();

    public async Task<Administrator?> GetAdministratorByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or whitespace.", nameof(username));

        return await _administrators
            .FirstOrDefaultAsync(x => x.Username.Value == username, cancellationToken);
    }
}
