using MassTransit;
using NotificationService.Models;

namespace NotificationService.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreated>
{
    public Task Consume(ConsumeContext<UserCreated> context)
    {
        Console.WriteLine($"User created: {context.Message.Email}");
        return Task.CompletedTask;
    }
}