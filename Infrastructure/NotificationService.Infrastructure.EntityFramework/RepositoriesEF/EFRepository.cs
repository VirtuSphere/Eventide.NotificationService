using NotificationService.Domain.Base;
using NotificationService.Domain.Repositories.Abstractions.Base;
using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure.EntityFramework;

namespace NotificationService.Infrastructure.EntityFramework.RepositoriesEF;

public class EfRepository<TEntity, TId>(ApplicationDbContext context)
        : IRepository<TEntity, TId>
        where TEntity : Entity<TId>
        where TId : struct, IEquatable<TId>
{
    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Set<TEntity>()
        .ToListAsync(cancellationToken);

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken)
        => await context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);

    public async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
        => await context.Set<TEntity>().AnyAsync(x => EqualityComparer<TId>.Default.Equals(x.Id, id), cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        await context.Set<TEntity>().AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        context.Set<TEntity>().Update(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        context.Set<TEntity>().Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken)
    {
        var entity = await GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        await DeleteAsync(entity, cancellationToken);
        return true;
    }
}
