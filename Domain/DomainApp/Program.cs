using NotificationService.Domain;
using NotificationService.Domain.Enums;
using NotificationService.ValueObjects;

namespace DomainApp;

public class Program
{
    public static void Main(string[] args)
    {
        var administrator = new Administrator(Guid.NewGuid(), new Username("admin"));
        var recipientUser = new User(Guid.NewGuid(), new Username("user1"));
        var notification = administrator.CreateNotification(recipientUser, new Title("Game"), new Body("Body"), NotificationType.Success);
        recipientUser.MarkAsRead(notification);
        notification.MarkAsRead(DateTime.UtcNow);
        administrator.DeleteNotification(notification);

        Console.WriteLine(notification.ToString());

    }
}