using System;
using NotificationService.Domain.Base;
using NotificationService.ValueObjects;

namespace NotificationService.Domain;

public class RelatedEntity : Entity<Guid>
{
    protected RelatedEntity() : base()
    {
    }

    public RelatedEntity(Guid id, RelatedEntityDisplayName? displayName = null, ExternalUrl? externalUrl = null) : base(id)
    {
        DisplayName = displayName;
        ExternalUrl = externalUrl;
    }

    public RelatedEntityDisplayName? DisplayName { get; private set; }

    public ExternalUrl? ExternalUrl { get; private set; }

    public void UpdateDisplay(RelatedEntityDisplayName? displayName, ExternalUrl? externalUrl)
    {
        DisplayName = displayName;
        ExternalUrl = externalUrl;
    }

    public override string ToString()
        => DisplayName?.ToString() ?? Id.ToString();

    public override bool Equals(object? obj)
        => obj is RelatedEntity other && Id.Equals(other.Id);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(RelatedEntity? left, RelatedEntity? right)
        => ReferenceEquals(left, right) || (left is not null && right is not null && left.Id.Equals(right.Id));

    public static bool operator !=(RelatedEntity? left, RelatedEntity? right)
        => !(left == right);
}
