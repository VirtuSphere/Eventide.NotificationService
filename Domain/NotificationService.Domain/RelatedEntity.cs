using NotificationService.Domain.Base;

namespace NotificationService.Domain;

public class RelatedEntity : Entity<Guid>
{
    protected RelatedEntity(): base()
    {
    }
    public RelatedEntity(Guid id) : base(id)
    {
    }
}
