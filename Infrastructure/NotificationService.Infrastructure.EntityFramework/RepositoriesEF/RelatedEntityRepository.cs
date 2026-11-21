using NotificationService.Domain;
using NotificationService.Domain.Repositories.Abstractions.Repositories;

namespace NotificationService.Infrastructure.EntityFramework.RepositoriesEF;

public class EfRelatedEntityRepository(ApplicationDbContext context)
    : EfRepository<RelatedEntity, Guid>(context), IRelatedEntityRepository
{
}
