using AutoMapper;
using NotificationService.Application.Models.Notification;
using NotificationService.Application.Services.Abstractions;
using NotificationService.Domain;
using NotificationService.Domain.Repositories.Abstractions.Repositories;
using NotificationService.ValueObjects;

namespace NotificationService.Application.Services;

public sealed class NotificationApplicationService(
    INotificationRepository repository,
    IUserRepository userRepository,
    IAdministratorRepository administratorRepository,
    IRelatedEntityRepository relatedEntityRepository,
    IMapper mapper) : INotificationApplicationService
{
    public async Task<IEnumerable<NotificationModel>> GetModelsAsync(
        Guid? userId = null,
        Guid? administratorId = null,
        bool onlyUnread = false,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Notification> notifications;
        if (administratorId.HasValue)
            notifications = await repository.GetByAdministratorAsync(administratorId.Value, cancellationToken);
        else if (userId.HasValue)
            notifications = await repository.GetByUserAsync(userId.Value, onlyUnread, cancellationToken);
        else
            notifications = await repository.GetAllAsync(cancellationToken);

        return notifications.Select(mapper.Map<NotificationModel>);
    }

    public async Task<NotificationModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(id, cancellationToken);
        return notification is null ? null : mapper.Map<NotificationModel>(notification);
    }

    public async Task<NotificationModel?> CreateModelAsync(CreateNotificationModel information, CancellationToken cancellationToken = default)
    {
        var recipient = await userRepository.GetByIdAsync(information.RecipientUserId, cancellationToken);
        if (recipient is null)
            return null;

        Administrator? administrator = null;
        if (information.AdministratorId.HasValue)
        {
            administrator = await administratorRepository.GetByIdAsync(information.AdministratorId.Value, cancellationToken);
            if (administrator is null)
                return null;
        }

        RelatedEntity? relatedEntity = null;
        if (information.RelatedEntityId.HasValue)
        {
            relatedEntity = await relatedEntityRepository.GetByIdAsync(information.RelatedEntityId.Value, cancellationToken);
            if (relatedEntity is null)
                return null;
        }

        RelatedEntityTypeName? relatedType = string.IsNullOrWhiteSpace(information.RelatedEntityType)
            ? null
            : new RelatedEntityTypeName(information.RelatedEntityType);

        var notification = new Notification(
            recipient,
            new Title(information.Title),
            new Body(information.Body),
            information.Type,
            DateTime.UtcNow,
            administrator,
            relatedType,
            relatedEntity);
        await repository.AddAsync(notification, cancellationToken);
        return mapper.Map<NotificationModel>(notification);
    }

    public async Task<bool> UpdateModelAsync(Guid id, CreateNotificationModel information, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(id, cancellationToken);
        var recipient = await userRepository.GetByIdAsync(information.RecipientUserId, cancellationToken);
        if (notification is null || recipient is null)
            return false;

        RelatedEntity? relatedEntity = null;
        if (information.RelatedEntityId.HasValue)
        {
            relatedEntity = await relatedEntityRepository.GetByIdAsync(information.RelatedEntityId.Value, cancellationToken);
            if (relatedEntity is null)
                return false;
        }

        RelatedEntityTypeName? relatedType = string.IsNullOrWhiteSpace(information.RelatedEntityType)
            ? null
            : new RelatedEntityTypeName(information.RelatedEntityType);

        notification.Update(
            recipient,
            new Title(information.Title),
            new Body(information.Body),
            information.Type,
            relatedType,
            relatedEntity);
        await repository.UpdateAsync(notification, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(id, cancellationToken);
        if (notification is null)
            return false;

        await repository.DeleteAsync(notification, cancellationToken);
        return true;
    }

    public async Task<bool> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(id, cancellationToken);
        if (notification is null || !notification.MarkAsRead(DateTime.UtcNow))
            return false;

        await repository.UpdateAsync(notification, cancellationToken);
        return true;
    }

    public async Task<bool> MarkAsUnreadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(id, cancellationToken);
        if (notification is null || !notification.MarkAsUnread())
            return false;

        await repository.UpdateAsync(notification, cancellationToken);
        return true;
    }

    public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
        => repository.GetUnreadCountAsync(userId, cancellationToken);
}
