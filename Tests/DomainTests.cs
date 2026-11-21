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
        Assert.True(first == second);
        Assert.False(first != second);
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
        Assert.False(administrator.UpdateNotification(new Notification(
            recipient,
            new Title("Other"),
            new Body("Other body"),
            NotificationType.Error,
            DateTime.UtcNow)));
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
}
