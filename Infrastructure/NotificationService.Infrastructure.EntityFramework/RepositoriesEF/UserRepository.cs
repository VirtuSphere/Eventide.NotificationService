using NotificationService.Domain;
using NotificationService.Domain.Repositories.Abstractions;
using NotificationService.Domain.Repositories.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Infrastructure.EntityFramework.RepositoriesEF;

public class EfUserRepository(ApplicationDbContext context)
    : EfRepository<User, Guid>(context), IUserRepository
{
    private readonly DbSet<User> _users = context.Set<User>();

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or whitespace.", nameof(username));

        return await _users
            .FirstOrDefaultAsync(x => x.Username.Value == username, cancellationToken);
    }
}