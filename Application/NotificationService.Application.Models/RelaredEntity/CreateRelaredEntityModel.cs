using NotificationService.Application.Models.Base;

namespace NotificationService.Application.Models.RelatedEntity;

public sealed record CreateRelatedEntityModel(
    string? DisplayName,
    string? ExternalUrl) : ICreateModel;