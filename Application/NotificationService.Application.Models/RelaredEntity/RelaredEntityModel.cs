using NotificationService.Application.Models.Base;

namespace NotificationService.Application.Models.RelatedEntity;

public sealed record RelatedEntityModel(
    Guid Id,
    string? DisplayName,
    string? ExternalUrl) : IModel<Guid>;