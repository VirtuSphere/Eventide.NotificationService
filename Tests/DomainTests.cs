using NotificationService.Domain;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Exceptions;
using NotificationService.ValueObjects;
using NotificationService.ValueObjects.Exceptions;
using NotificationService.ValueObjects.Validators;

namespace NotificationService.Tests;

public class DomainTests
{
    [Fact]
    public void UserCanChangeUsernameAndMarkNotificationStates()
    {
        var user = new User(Guid.NewGuid(), new Username("alice"));
        var notification = CreateNotification(user);

        user.ChangeUsername(new Username("bob"));

        Assert.True(user.MarkAsRead(notification));
        Assert.True(notification.IsRead);
        Assert.True(user.MarkAsUnread(notification));
        Assert.False(notification.IsRead);
        Assert.False(user.MarkAsUnread(notification));
        Assert.True(user.MarkAsRead(notification));
    }

    [Fact]
    public void AdministratorCreatesDeletesAndTracksNotifications()
    {
        var administrator = new Administrator(Guid.NewGuid(), new Username("admin"));
        var notification = administrator.CreateNotification(
            new User(Guid.NewGuid(), new Username("user")),
            new Title("Title"),
            new Body("Body"),
            NotificationType.Success);

        Assert.Contains(notification, administrator.Notifications);
        Assert.True(administrator.DeleteNotification(notification));
        Assert.DoesNotContain(notification, administrator.Notifications);
        Assert.False(administrator.DeleteNotification(notification));
    }

    [Fact]
    public void NotificationUpdateChangesFieldsAndRejectsNulls()
    {
        var recipient = new User(Guid.NewGuid(), new Username("user"));
        var notification = CreateNotification(recipient);
        var newRecipient = new User(Guid.NewGuid(), new Username("new-user"));

        Assert.True(notification.Update(
            newRecipient,
            new Title("New title"),
            new Body("New body"),
            NotificationType.Error));
        Assert.Equal(newRecipient.Id, notification.RecipientUser.Id);
        Assert.Equal("New title", notification.Title.Value);
        Assert.False(notification.Update(
            newRecipient,
            new Title("New title"),
            new Body("New body"),
            NotificationType.Error));
        Assert.Throws<ArgumentNullValueException>(() =>
            notification.Update(null!, new Title("Title"), new Body("Body"), NotificationType.Info));
    }

    [Fact]
    public void NotificationReadStateTransitions()
    {
        var notification = CreateNotification(new User(Guid.NewGuid(), new Username("user")));

        Assert.True(notification.MarkAsRead(DateTime.UtcNow));
        Assert.NotNull(notification.ReadAt);
        Assert.False(notification.MarkAsRead(DateTime.UtcNow));
        Assert.True(notification.MarkAsUnread());
        Assert.Null(notification.ReadAt);
        Assert.False(notification.MarkAsUnread());
    }

    [Fact]
    public void RelatedEntityUpdatesAndComparesById()
    {
        var id = Guid.NewGuid();
        var first = new RelatedEntity(id, new RelatedEntityDisplayName("First"));
        var second = new RelatedEntity(id);

        first.UpdateDisplay(new RelatedEntityDisplayName("Second"), new ExternalUrl("https://example.com"));

        Assert.Equal(first, second);
        Assert.Equal(id, first.Id);
        Assert.Equal("Second", first.DisplayName!.Value);
        Assert.Equal("https://example.com", first.ExternalUrl!.Value);
        Assert.Equal("Second", first.ToString());
        Assert.Equal(id.GetHashCode(), first.GetHashCode());
        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void EntityAndRelatedEntitySupportProtectedPersistenceConstructors()
    {
        var entity = new TestEntity();
        var relatedEntity = new TestRelatedEntity();

        Assert.Equal(Guid.Empty, entity.Id);
        Assert.Equal(Guid.Empty, relatedEntity.Id);
        Assert.Equal(Guid.Empty.ToString(), relatedEntity.ToString());
        Assert.Equal(Guid.Empty.GetHashCode(), relatedEntity.GetHashCode());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ab")]
    [InlineData("abcdefghijklmnopqrstuvwxyzabcd1")]
    public void UsernameRejectsInvalidValues(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => new Username(value));
        else if (value.Length < UsernameValidator.MIN_LENGTH)
            Assert.Throws<ArgumentShortValueException>(() => new Username(value));
        else
            Assert.Throws<ArgumentLongValueException>(() => new Username(value));
    }

    [Fact]
    public void ValueObjectsSupportEqualityAndToString()
    {
        var first = new Title("Title");
        var second = new Title("Title");
        var body = new Body("Body");

        Assert.True(first.Equals(second));
        Assert.Equal(first, second);
        Assert.Equal("Title", first.ToString());
        Assert.Equal("Body", body.ToString());
        Assert.True(first.Equals((object)first));
        Assert.False(first.Equals(null));
        Assert.False(first.Equals(new Username("Title")));
    }

    [Fact]
    public void ValueObjectRejectsMissingValidator()
    {
        Assert.Throws<ValidatorNullException>(() => new TestValueObject(null, "value"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ab")]
    public void TitleRejectsInvalidValues(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => new Title(value));
        else
            Assert.Throws<ArgumentShortValueException>(() => new Title(value));
    }

    [Fact]
    public void BodyRejectsEmptyAndTooLongValues()
    {
        Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => new Body(""));
        Assert.Throws<ArgumentLongValueException>(() => new Body(new string('x', 1001)));
    }

    [Fact]
    public void RelatedEntityValueObjectsValidateValues()
    {
        Assert.Throws<ArgumentNullOrWhiteSpaceException>(
            () => new RelatedEntityDisplayName(""));
        Assert.Throws<ArgumentNullOrWhiteSpaceException>(
            () => new RelatedEntityTypeName(""));
        Assert.Throws<ArgumentNullOrWhiteSpaceException>(
            () => new ExternalUrl(""));
    }

    [Fact]
    public void ExternalUrlValidatesLengthAndFormat()
    {
        Assert.Equal("https://example.com", new ExternalUrl("https://example.com").Value);
        Assert.Throws<ArgumentLongValueException>(
            () => new ExternalUrl("https://example.com/" + new string('x', 2000)));
        Assert.Throws<ArgumentException>(
            () => new ExternalUrl("not a uri"));
    }

    [Fact]
    public void RelatedEntityNamesValidateMinimumAndMaximumLength()
    {
        Assert.Throws<ArgumentLongValueException>(
            () => new RelatedEntityDisplayName(new string('x', 201)));
        Assert.Throws<ArgumentShortValueException>(
            () => new RelatedEntityTypeName("x"));
        Assert.Throws<ArgumentLongValueException>(
            () => new RelatedEntityTypeName(new string('x', 101)));
        Assert.Equal("Quest", new RelatedEntityDisplayName("Quest").Value);
        Assert.Equal("Match", new RelatedEntityTypeName("Match").Value);
    }

    [Fact]
    public void AdministratorChangeUsernameAndUpdateNotification()
    {
        var administrator = new Administrator(Guid.NewGuid(), new Username("before"));
        var recipient = new User(Guid.NewGuid(), new Username("recipient"));
        var notification = administrator.CreateNotification(
            recipient,
            new Title("Title"),
            new Body("Body"),
            NotificationType.Info);

        administrator.ChangeUsername(new Username("after"));
        var updated = administrator.UpdateNotification(
            notification,
            new Title("Updated"),
            new Body("Updated body"),
            NotificationType.Warning);

        Assert.Equal("after", administrator.Username.Value);
        Assert.True(updated);
        Assert.Equal("Updated", notification.Title.Value);
        Assert.Equal("Updated body", notification.Body.Value);
        Assert.Equal(NotificationType.Warning, notification.Type);
        Assert.False(administrator.UpdateNotification(notification));
        Assert.False(administrator.UpdateNotification(new Notification(
            recipient,
            new Title("Other"),
            new Body("Other body"),
            NotificationType.Error,
            DateTime.UtcNow)));
    }

    [Fact]
    public void NotificationPersistenceConstructorRestoresStateAndRejectsMissingValues()
    {
        var id = Guid.NewGuid();
        var recipient = new User(Guid.NewGuid(), new Username("recipient"));
        var administrator = new Administrator(Guid.NewGuid(), new Username("admin"));
        var relatedEntity = new RelatedEntity(Guid.NewGuid());
        var readAt = DateTime.UtcNow;
        var notification = new PersistedNotification(
            id,
            recipient,
            new Title("Title"),
            new Body("Body"),
            NotificationType.Success,
            readAt,
            true,
            readAt,
            administrator,
            new RelatedEntityTypeName("Task"),
            relatedEntity);

        Assert.Equal(id, notification.Id);
        Assert.Same(recipient, notification.RecipientUser);
        Assert.Equal("Title", notification.Title.Value);
        Assert.Equal("Body", notification.Body.Value);
        Assert.Equal(NotificationType.Success, notification.Type);
        Assert.True(notification.IsRead);
        Assert.Equal(readAt, notification.ReadAt);
        Assert.Same(administrator, notification.Administrator);
        Assert.Equal("Task", notification.RelatedEntityType!.Value);
        Assert.Same(relatedEntity, notification.RelatedEntity);
        Assert.Equal(readAt, notification.CreatedAt);

        Assert.Throws<ArgumentNullValueException>(() => new PersistedNotification(
            id, null, new Title("Title"), new Body("Body"), NotificationType.Info, readAt));
        Assert.Throws<ArgumentNullValueException>(() => new PersistedNotification(
            id, recipient, null, new Body("Body"), NotificationType.Info, readAt));
        Assert.Throws<ArgumentNullValueException>(() => new PersistedNotification(
            id, recipient, new Title("Title"), null, NotificationType.Info, readAt));
    }

    [Fact]
    public void ValueObjectsCoverHashEqualityAndOperators()
    {
        var left = new Username("alice");
        var same = new Username("alice");
        var right = new Username("bob");

        Assert.Equal(left.GetHashCode(), same.GetHashCode());
        Assert.True(left == same);
        Assert.False(left == right);
        Assert.True(left != right);
        Assert.True(left.Equals((object)same));
    }

    private static Notification CreateNotification(User recipient)
        => new(
            recipient,
            new Title("Title"),
            new Body("Body"),
            NotificationType.Info,
            DateTime.UtcNow);

    private sealed class TestEntity : NotificationService.Domain.Base.Entity<Guid>
    {
        public TestEntity() : base()
        {
        }
    }

    private sealed class TestRelatedEntity : RelatedEntity
    {
        public TestRelatedEntity() : base()
        {
        }
    }

    private sealed class TestValueObject(
        NotificationService.ValueObjects.Base.IValidator<string>? validator,
        string value)
        : NotificationService.ValueObjects.Base.ValueObject<string>(validator!, value);

    private sealed class PersistedNotification : Notification
    {
        public PersistedNotification(
            Guid id,
            User? recipientUser,
            Title? title,
            Body? body,
            NotificationType type,
            DateTime createdAt,
            bool isRead = false,
            DateTime? readAt = null,
            Administrator? administrator = null,
            RelatedEntityTypeName? relatedEntityType = null,
            RelatedEntity? relatedEntity = null)
            : base(
                id,
                recipientUser!,
                title!,
                body!,
                type,
                createdAt,
                isRead,
                readAt,
                administrator,
                relatedEntityType,
                relatedEntity)
        {
        }
    }
}
