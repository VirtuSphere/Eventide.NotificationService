using AutoMapper;
using Moq;
using NotificationService.Application.Models.Administrator;
using NotificationService.Application.Models.Notification;
using NotificationService.Application.Models.RelatedEntity;
using NotificationService.Application.Models.User;
using NotificationService.Application.Services;
using NotificationService.Domain;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Repositories.Abstractions.Repositories;
using NotificationService.ValueObjects;

namespace NotificationService.Tests;

public class ApplicationServicesTests
{
    private readonly IMapper _mapper;

    public ApplicationServicesTests()
    {
        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<UserModel>(It.IsAny<object>()))
            .Returns((object source) =>
            {
                var user = (User)source;
                return new UserModel(user.Id, user.Username.Value);
            });
        mapper.Setup(x => x.Map<AdministratorModel>(It.IsAny<object>()))
            .Returns((object source) =>
            {
                var administrator = (Administrator)source;
                return new AdministratorModel(administrator.Id, administrator.Username.Value);
            });
        mapper.Setup(x => x.Map<RelatedEntityModel>(It.IsAny<object>()))
            .Returns((object source) =>
            {
                var entity = (RelatedEntity)source;
                return new RelatedEntityModel(entity.Id, entity.DisplayName?.Value, entity.ExternalUrl?.Value);
            });
        mapper.Setup(x => x.Map<NotificationModel>(It.IsAny<object>()))
            .Returns((object source) =>
            {
                var notification = (Notification)source;
                return new NotificationModel(
                    notification.Id,
                    notification.RecipientUser.Id,
                    notification.Administrator?.Id,
                    notification.Title.Value,
                    notification.Body.Value,
                    notification.Type,
                    notification.IsRead,
                    notification.ReadAt,
                    notification.RelatedEntityType?.Value,
                    notification.RelatedEntity?.Id,
                    notification.CreatedAt);
            });
        _mapper = mapper.Object;
    }

    [Fact]
    public async Task UserServiceReturnsUsersAndCreatesUser()
    {
        var repository = new Mock<IUserRepository>();
        var id = Guid.NewGuid();
        var user = new User(id, new Username("Alex"));
        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { user });
        repository.Setup(x => x.GetUserByUsernameAsync("new-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var service = new UserApplicationService(repository.Object, _mapper);

        var users = await service.GetModelsAsync();
        var created = await service.CreateModelAsync(new CreateUserModel("new-user"));

        Assert.Equal("Alex", Assert.Single(users).Username);
        Assert.NotNull(created);
        Assert.Equal("new-user", created.Username);
        repository.Verify(x => x.AddAsync(It.Is<User>(u => u.Username.Value == "new-user"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UserServiceRejectsDuplicateAndMissingUpdate()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetUserByUsernameAsync("taken", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User(Guid.NewGuid(), new Username("taken")));
        repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var service = new UserApplicationService(repository.Object, _mapper);

        Assert.Null(await service.CreateModelAsync(new CreateUserModel("taken")));
        Assert.False(await service.UpdateModelAsync(new UserModel(Guid.NewGuid(), "missing"), default));
    }

    [Fact]
    public async Task AdministratorServiceUpdatesAndDeletes()
    {
        var repository = new Mock<IAdministratorRepository>();
        var administrator = new Administrator(Guid.NewGuid(), new Username("admin"));
        repository.Setup(x => x.GetByIdAsync(administrator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrator);
        repository.Setup(x => x.GetAdministratorByUsernameAsync("renamed", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Administrator?)null);

        var service = new AdministratorApplicationService(repository.Object, _mapper);

        Assert.True(await service.UpdateModelAsync(
            new AdministratorModel(administrator.Id, "renamed")));
        Assert.True(await service.DeleteModelAsync(administrator.Id));
        repository.Verify(x => x.UpdateAsync(administrator, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.DeleteAsync(administrator, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RelatedEntityServiceMapsAndUpdatesEntity()
    {
        var repository = new Mock<IRelatedEntityRepository>();
        var entity = new RelatedEntity(
            Guid.NewGuid(),
            new RelatedEntityDisplayName("Quest"),
            new ExternalUrl("https://example.com/quest"));
        repository.Setup(x => x.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var service = new RelatedEntityApplicationService(repository.Object, _mapper);
        var model = await service.GetModelByIdAsync(entity.Id);
        var updated = await service.UpdateModelAsync(
            new RelatedEntityModel(entity.Id, "Updated", "https://example.com/updated"));

        Assert.Equal("Quest", model!.DisplayName);
        Assert.True(updated);
        Assert.Equal("Updated", entity.DisplayName!.Value);
        Assert.Equal("https://example.com/updated", entity.ExternalUrl!.Value);
    }

    [Fact]
    public async Task NotificationServiceCreatesAndMarksNotification()
    {
        var notifications = new Mock<INotificationRepository>();
        var users = new Mock<IUserRepository>();
        var administrators = new Mock<IAdministratorRepository>();
        var relatedEntities = new Mock<IRelatedEntityRepository>();
        var recipient = new User(Guid.NewGuid(), new Username("recipient"));
        users.Setup(x => x.GetByIdAsync(recipient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipient);

        var service = new NotificationApplicationService(
            notifications.Object,
            users.Object,
            administrators.Object,
            relatedEntities.Object,
            _mapper);
        var model = new CreateNotificationModel(
            recipient.Id,
            null,
            "Test title",
            "Test body",
            NotificationType.Info,
            null,
            null);

        var created = await service.CreateModelAsync(model);
        Assert.NotNull(created);

        Notification? stored = null;
        notifications.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => stored);
        notifications.Setup(x => x.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Callback<Notification, CancellationToken>((notification, _) => stored = notification)
            .Returns(Task.CompletedTask);

        created = await service.CreateModelAsync(model);
        Assert.NotNull(created);
        Assert.True(await service.MarkAsReadAsync(created!.Id));
        Assert.True(await service.MarkAsUnreadAsync(created.Id));
    }

    [Fact]
    public async Task NotificationServiceReturnsFilteredCollectionsAndUnreadCount()
    {
        var notifications = new Mock<INotificationRepository>();
        var users = new Mock<IUserRepository>();
        var administrators = new Mock<IAdministratorRepository>();
        var relatedEntities = new Mock<IRelatedEntityRepository>();
        var userId = Guid.NewGuid();
        var notification = new Notification(
            new User(userId, new Username("recipient")),
            new Title("Title"),
            new Body("Body"),
            NotificationType.Warning,
            DateTime.UtcNow);
        notifications.Setup(x => x.GetByUserAsync(userId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { notification });
        notifications.Setup(x => x.GetUnreadCountAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var service = new NotificationApplicationService(
            notifications.Object,
            users.Object,
            administrators.Object,
            relatedEntities.Object,
            _mapper);

        var models = await service.GetModelsAsync(userId, null, true);
        Assert.Single(models);
        Assert.Equal(3, await service.GetUnreadCountAsync(userId));
    }

    [Fact]
    public async Task UserServiceCoversUpdateDeleteAndMissingBranches()
    {
        var repository = new Mock<IUserRepository>();
        var user = new User(Guid.NewGuid(), new Username("alice"));
        repository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        repository.Setup(x => x.GetUserByUsernameAsync("bob", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        var service = new UserApplicationService(repository.Object, _mapper);

        Assert.True(await service.UpdateModelAsync(new UserModel(user.Id, "bob")));
        Assert.True(await service.DeleteModelAsync(user.Id));
        Assert.False(await service.DeleteModelAsync(Guid.NewGuid()));
        Assert.False(await service.UpdateModelAsync(new UserModel(Guid.NewGuid(), "bob")));
    }

    [Fact]
    public async Task UserServiceGetsUserAndRejectsUsernameOwnedByAnotherUser()
    {
        var repository = new Mock<IUserRepository>();
        var user = new User(Guid.NewGuid(), new Username("alice"));
        var otherUser = new User(Guid.NewGuid(), new Username("taken"));
        repository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        repository.Setup(x => x.GetUserByUsernameAsync("alice", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        repository.Setup(x => x.GetUserByUsernameAsync("taken", It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherUser);

        var service = new UserApplicationService(repository.Object, _mapper);

        Assert.Equal("alice", (await service.GetModelByIdAsync(user.Id))!.Username);
        Assert.Null(await service.GetModelByIdAsync(Guid.NewGuid()));
        Assert.True(await service.UpdateModelAsync(new UserModel(user.Id, "alice")));
        Assert.False(await service.UpdateModelAsync(new UserModel(user.Id, "taken")));
    }

    [Fact]
    public async Task AdministratorServiceCoversCreateListGetAndMissingBranches()
    {
        var repository = new Mock<IAdministratorRepository>();
        var administrator = new Administrator(Guid.NewGuid(), new Username("admin"));
        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { administrator });
        repository.Setup(x => x.GetByIdAsync(administrator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrator);
        repository.Setup(x => x.GetAdministratorByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrator);
        repository.Setup(x => x.GetAdministratorByUsernameAsync("new-admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Administrator?)null);

        var service = new AdministratorApplicationService(repository.Object, _mapper);

        Assert.Single(await service.GetModelsAsync());
        Assert.NotNull(await service.GetModelByIdAsync(administrator.Id));
        Assert.NotNull(await service.CreateModelAsync(new CreateAdministratorModel("new-admin")));
        Assert.Null(await service.CreateModelAsync(new CreateAdministratorModel("admin")));
        Assert.False(await service.DeleteModelAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task AdministratorServiceGetsByUsernameAndRejectsConflictingUpdates()
    {
        var repository = new Mock<IAdministratorRepository>();
        var administrator = new Administrator(Guid.NewGuid(), new Username("admin"));
        var otherAdministrator = new Administrator(Guid.NewGuid(), new Username("taken"));
        repository.Setup(x => x.GetAdministratorByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrator);
        repository.Setup(x => x.GetAdministratorByUsernameAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Administrator?)null);
        repository.Setup(x => x.GetAdministratorByUsernameAsync("taken", It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherAdministrator);
        repository.Setup(x => x.GetByIdAsync(administrator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrator);
        repository.Setup(x => x.GetByIdAsync(It.Is<Guid>(id => id != administrator.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Administrator?)null);

        var service = new AdministratorApplicationService(repository.Object, _mapper);

        Assert.Equal("admin", (await service.GetAdministratorByUsernameAsync("admin"))!.Username);
        Assert.Null(await service.GetAdministratorByUsernameAsync("missing"));
        Assert.False(await service.UpdateModelAsync(new AdministratorModel(Guid.NewGuid(), "new-name")));
        Assert.False(await service.UpdateModelAsync(new AdministratorModel(administrator.Id, "taken")));
    }

    [Fact]
    public async Task RelatedEntityServiceCoversCreateDeleteAndMissingBranches()
    {
        var repository = new Mock<IRelatedEntityRepository>();
        var entity = new RelatedEntity(Guid.NewGuid(), new RelatedEntityDisplayName("entity"));
        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { entity });
        repository.Setup(x => x.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var service = new RelatedEntityApplicationService(repository.Object, _mapper);

        Assert.Single(await service.GetModelsAsync());
        Assert.NotNull(await service.CreateModelAsync(
            new CreateRelatedEntityModel("new", "https://example.com")));
        Assert.True(await service.DeleteModelAsync(entity.Id));
        Assert.False(await service.DeleteModelAsync(Guid.NewGuid()));
        Assert.False(await service.UpdateModelAsync(
            new RelatedEntityModel(Guid.NewGuid(), "missing", null)));
    }

    [Fact]
    public async Task RelatedEntityServiceTreatsBlankOptionalFieldsAsNull()
    {
        var repository = new Mock<IRelatedEntityRepository>();
        RelatedEntity? stored = null;
        repository.Setup(x => x.AddAsync(It.IsAny<RelatedEntity>(), It.IsAny<CancellationToken>()))
            .Callback<RelatedEntity, CancellationToken>((entity, _) => stored = entity)
            .Returns(Task.CompletedTask);
        repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => stored);

        var service = new RelatedEntityApplicationService(repository.Object, _mapper);
        var created = await service.CreateModelAsync(new CreateRelatedEntityModel(" ", null));

        Assert.NotNull(created);
        Assert.Null(created.DisplayName);
        Assert.Null(created.ExternalUrl);
        Assert.True(await service.UpdateModelAsync(new RelatedEntityModel(created.Id, null, " ")));
        Assert.Null(stored!.DisplayName);
        Assert.Null(stored.ExternalUrl);
    }

    [Fact]
    public async Task NotificationServiceCoversUpdateDeleteAndMissingEntities()
    {
        var notifications = new Mock<INotificationRepository>();
        var users = new Mock<IUserRepository>();
        var administrators = new Mock<IAdministratorRepository>();
        var relatedEntities = new Mock<IRelatedEntityRepository>();
        var recipient = new User(Guid.NewGuid(), new Username("recipient"));
        var notification = new Notification(
            recipient,
            new Title("old"),
            new Body("old body"),
            NotificationType.Info,
            DateTime.UtcNow);
        users.Setup(x => x.GetByIdAsync(recipient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipient);
        notifications.Setup(x => x.GetByIdAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        var service = new NotificationApplicationService(
            notifications.Object,
            users.Object,
            administrators.Object,
            relatedEntities.Object,
            _mapper);
        var model = new CreateNotificationModel(
            recipient.Id,
            null,
            "new",
            "new body",
            NotificationType.Warning,
            null,
            null);

        Assert.True(await service.UpdateModelAsync(notification.Id, model));
        Assert.True(await service.DeleteModelAsync(notification.Id));
        Assert.False(await service.DeleteModelAsync(Guid.NewGuid()));
        Assert.False(await service.MarkAsReadAsync(Guid.NewGuid()));
        Assert.False(await service.MarkAsUnreadAsync(Guid.NewGuid()));
        Assert.False(await service.UpdateModelAsync(Guid.NewGuid(), model));
        Assert.Null(await service.GetModelByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task NotificationServiceCoversAdministratorFilterAndMissingReferences()
    {
        var notifications = new Mock<INotificationRepository>();
        var users = new Mock<IUserRepository>();
        var administrators = new Mock<IAdministratorRepository>();
        var relatedEntities = new Mock<IRelatedEntityRepository>();
        var administratorId = Guid.NewGuid();
        var notification = new Notification(
            new User(Guid.NewGuid(), new Username("recipient")),
            new Title("title"),
            new Body("body"),
            NotificationType.Info,
            DateTime.UtcNow);
        notifications.Setup(x => x.GetByAdministratorAsync(administratorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { notification });
        notifications.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { notification });

        var service = new NotificationApplicationService(
            notifications.Object,
            users.Object,
            administrators.Object,
            relatedEntities.Object,
            _mapper);

        Assert.Single(await service.GetModelsAsync(null, administratorId));
        Assert.Single(await service.GetModelsAsync());
        Assert.Null(await service.CreateModelAsync(new CreateNotificationModel(
            Guid.NewGuid(), null, "title", "body", NotificationType.Info, null, null)));
    }

    [Fact]
    public async Task NotificationServiceHandlesOptionalRelationsAndMissingReferences()
    {
        var notifications = new Mock<INotificationRepository>();
        var users = new Mock<IUserRepository>();
        var administrators = new Mock<IAdministratorRepository>();
        var relatedEntities = new Mock<IRelatedEntityRepository>();
        var recipient = new User(Guid.NewGuid(), new Username("recipient"));
        var administrator = new Administrator(Guid.NewGuid(), new Username("admin"));
        var relatedEntity = new RelatedEntity(Guid.NewGuid());
        users.Setup(x => x.GetByIdAsync(recipient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipient);
        administrators.Setup(x => x.GetByIdAsync(administrator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrator);
        relatedEntities.Setup(x => x.GetByIdAsync(relatedEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(relatedEntity);
        Notification? stored = null;
        notifications.Setup(x => x.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Callback<Notification, CancellationToken>((notification, _) => stored = notification)
            .Returns(Task.CompletedTask);
        notifications.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => stored);

        var service = new NotificationApplicationService(
            notifications.Object,
            users.Object,
            administrators.Object,
            relatedEntities.Object,
            _mapper);

        var missingAdministrator = await service.CreateModelAsync(new CreateNotificationModel(
            recipient.Id, Guid.NewGuid(), "title", "body", NotificationType.Info, null, null));
        var missingRelatedEntity = await service.CreateModelAsync(new CreateNotificationModel(
            recipient.Id, null, "title", "body", NotificationType.Info, null, Guid.NewGuid()));
        Assert.Null(missingAdministrator);
        Assert.Null(missingRelatedEntity);

        var created = await service.CreateModelAsync(new CreateNotificationModel(
            recipient.Id,
            administrator.Id,
            "title",
            "body",
            NotificationType.Info,
            "Task",
            relatedEntity.Id));
        Assert.NotNull(created);
        Assert.Equal(administrator.Id, stored!.Administrator!.Id);
        Assert.Equal(relatedEntity.Id, stored.RelatedEntity!.Id);

        Assert.False(await service.UpdateModelAsync(stored.Id, new CreateNotificationModel(
            recipient.Id,
            null,
            "title",
            "body",
            NotificationType.Info,
            "Task",
            Guid.NewGuid())));
    }
}
