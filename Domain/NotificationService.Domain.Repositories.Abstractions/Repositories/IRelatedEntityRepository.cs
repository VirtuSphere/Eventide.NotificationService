using NotificationService.Domain.Repositories.Abstractions.Base;
using NotificationService.Domain;

namespace NotificationService.Domain.Repositories.Abstractions.Repositories;

public interface IRelatedEntityRepository : IRepository<RelatedEntity, Guid>
{
}
