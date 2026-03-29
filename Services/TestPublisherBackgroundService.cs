using MassTransit;
using Microsoft.Extensions.Hosting;
using NotificationService.Models;
using System.Threading;
using System.Threading.Tasks;

public class TestPublisherBackgroundService : BackgroundService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public TestPublisherBackgroundService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Дадим MassTransit запуститься
        await Task.Delay(2000, stoppingToken);

        await _publishEndpoint.Publish(new UserCreated
        {
            Email = "test@example.com"
        });
    }
}
