using NotificationService.Application.Models.RelatedEntity;
using NotificationService.Application.Services.Abstractions.Base;

namespace NotificationService.Application.Services.Abstractions;

public interface IRelatedEntityApplicationService : IApplicationService<RelatedEntityModel, CreateRelatedEntityModel, Guid>;
