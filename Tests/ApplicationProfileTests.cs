using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using NotificationService.Application.Models.Administrator;
using NotificationService.Application.Models.Notification;
using NotificationService.Application.Models.RelatedEntity;
using NotificationService.Application.Models.User;
using NotificationService.Application.Services.Mapping;
using NotificationService.Domain;
using NotificationService.Domain.Enums;
using NotificationService.ValueObjects;

namespace NotificationService.Tests;

public class ApplicationProfileTests
{
    [Fact]
    public void MapsAllDomainModelsToApplicationModels()
    {
        var configuration = new MapperConfiguration(
            config => config.AddProfile<ApplicationProfile>(),
            NullLoggerFactory.Instance);
        var mapper = configuration.CreateMapper();

        var user = new User(Guid.NewGuid(), new Username("recipient"));
        var administrator = new Administrator(Guid.NewGuid(), new Username("admin"));
        var relatedEntity = new RelatedEntity(
            Guid.NewGuid(),
            new RelatedEntityDisplayName("Task #42"),
            new ExternalUrl("https://example.com/tasks/42"));

        var userModel = mapper.Map<UserModel>(user);
        var administratorModel = mapper.Map<AdministratorModel>(administrator);
        var relatedEntityModel = mapper.Map<RelatedEntityModel>(relatedEntity);
        var emptyRelatedEntityModel = mapper.Map<RelatedEntityModel>(new RelatedEntity(Guid.NewGuid()));

        Assert.Equal(user.Id, userModel.Id);
        Assert.Equal("recipient", userModel.Username);
        Assert.Equal(administrator.Id, administratorModel.Id);
        Assert.Equal("admin", administratorModel.Username);
        Assert.Equal("Task #42", relatedEntityModel.DisplayName);
        Assert.Equal("https://example.com/tasks/42", relatedEntityModel.ExternalUrl);
        Assert.Null(emptyRelatedEntityModel.DisplayName);
        Assert.Null(emptyRelatedEntityModel.ExternalUrl);
    }

    [Fact]
    public void MapsNotificationWithAndWithoutOptionalRelations()
    {
        var configuration = new MapperConfiguration(
            config => config.AddProfile<ApplicationProfile>(),
            NullLoggerFactory.Instance);
        var mapper = configuration.CreateMapper();
        var recipient = new User(Guid.NewGuid(), new Username("recipient"));
        var administrator = new Administrator(Guid.NewGuid(), new Username("admin"));
        var relatedEntity = new RelatedEntity(Guid.NewGuid());
        var createdAt = DateTime.UtcNow;

        var notification = new Notification(
            recipient,
            new Title("A title"),
            new Body("A body"),
            NotificationType.Warning,
            createdAt,
            administrator,
            new RelatedEntityTypeName("Task"),
            relatedEntity);
        var model = mapper.Map<NotificationModel>(notification);

        Assert.Equal(notification.Id, model.Id);
        Assert.Equal(recipient.Id, model.RecipientUserId);
        Assert.Equal(administrator.Id, model.AdministratorId);
        Assert.Equal("A title", model.Title);
        Assert.Equal("A body", model.Body);
        Assert.Equal(NotificationType.Warning, model.Type);
        Assert.False(model.IsRead);
        Assert.Null(model.ReadAt);
        Assert.Equal("Task", model.RelatedEntityType);
        Assert.Equal(relatedEntity.Id, model.RelatedEntityId);
        Assert.Equal(createdAt, model.CreatedAt);

        var unlinkedNotification = new Notification(
            recipient,
            new Title("Another title"),
            new Body("Another body"),
            NotificationType.Info,
            createdAt);
        var unlinkedModel = mapper.Map<NotificationModel>(unlinkedNotification);

        Assert.Null(unlinkedModel.AdministratorId);
        Assert.Null(unlinkedModel.RelatedEntityType);
        Assert.Null(unlinkedModel.RelatedEntityId);
    }
}
