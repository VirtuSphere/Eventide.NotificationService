using NotificationService.Domain;
using NotificationService.Domain.Enums;
using NotificationService.ValueObjects;

namespace DomainApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Демонстрация сервиса уведомлений ===\n");

        // Создаем администратора
        Console.WriteLine("1. Создание администратора:");
        var admin = new Administrator(Guid.NewGuid(), new Username("admin_ivanov"));
        Console.WriteLine($"   Администратор: {admin.Username}\n");

        // Создаем пользователей
        Console.WriteLine("2. Создание пользователей:");
        var user1 = new User(Guid.NewGuid(), new Username("nikolay"));
        var user2 = new User(Guid.NewGuid(), new Username("marina"));
        var user3 = new User(Guid.NewGuid(), new Username("alex"));
        Console.WriteLine($"   - {user1.Username}");
        Console.WriteLine($"   - {user2.Username}");
        Console.WriteLine($"   - {user3.Username}\n");

        // Создаем уведомления разных типов
        Console.WriteLine("3. Создание уведомлений:");

        var notif1 = admin.CreateNotification(
            user1,
            new Title("Система обновлена"),
            new Body("Пожалуйста перезагрузите приложение для применения обновления"),
            NotificationType.Warning
        );
        Console.WriteLine($"   [{NotificationType.Warning}] {notif1.Title} -> {user1.Username}");

        var notif2 = admin.CreateNotification(
            user2,
            new Title("Добро пожаловать"),
            new Body("Ваш аккаунт успешно создан и готов к использованию"),
            NotificationType.Success
        );
        Console.WriteLine($"   [{NotificationType.Success}] {notif2.Title} -> {user2.Username}");

        var notif3 = admin.CreateNotification(
            user3,
            new Title("Вы получили сообщение"),
            new Body("У вас новое сообщение от администратора"),
            NotificationType.Info
        );
        Console.WriteLine($"   [{NotificationType.Info}] {notif3.Title} -> {user3.Username}");

        var notif4 = admin.CreateNotification(
            user1,
            new Title("Подозрительная активность"),
            new Body("Обнаружена попытка входа с неизвестного устройства"),
            NotificationType.Error
        );
        Console.WriteLine($"   [{NotificationType.Error}] {notif4.Title} -> {user1.Username}\n");

        // Показываем количество уведомлений
        Console.WriteLine($"4. Всего создано уведомлений: {admin.Notifications.Count}\n");

        // Пользователи отмечают уведомления как прочитанные
        Console.WriteLine("5. Отметить уведомления как прочитанные:");
        var result1 = user1.MarkAsRead(notif1);
        Console.WriteLine($"   {user1.Username} прочитал уведомление: {result1}");

        var result2 = user2.MarkAsRead(notif2);
        Console.WriteLine($"   {user2.Username} прочитал уведомление: {result2}");

        // Попытка прочитать уже прочитанное
        var result3 = user2.MarkAsRead(notif2);
        Console.WriteLine($"   {user2.Username} пытается прочитать еще раз: {result3} (не прочитано)\n");

        // Проверяем статусы
        Console.WriteLine("6. Статус уведомлений:");
        Console.WriteLine($"   Уведомление 1 - прочитано: {notif1.IsRead}");
        Console.WriteLine($"   Уведомление 2 - прочитано: {notif2.IsRead}");
        Console.WriteLine($"   Уведомление 3 - прочитано: {notif3.IsRead}\n");

        // Удаляем уведомление
        Console.WriteLine("7. Удаление уведомлений:");
        var deleteResult = admin.DeleteNotification(notif4);
        Console.WriteLine($"   Удалили уведомление 'Подозрительная активность': {deleteResult}");
        Console.WriteLine($"   Осталось уведомлений: {admin.Notifications.Count}\n");

        // Итоговая информация
        Console.WriteLine("8. Итоговая информация:");
        Console.WriteLine($"   Активных уведомлений: {admin.Notifications.Count}");
        Console.WriteLine($"   Прочитано: {admin.Notifications.Count(n => n.IsRead)}");
        Console.WriteLine($"   Не прочитано: {admin.Notifications.Count(n => !n.IsRead)}");

    }
}