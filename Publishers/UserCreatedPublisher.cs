using MassTransit;
using NotificationService.Models;

namespace NotificationService.Publishers;

public class UserCreatedPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public UserCreatedPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishUserCreated(string email)
    {
        await _publishEndpoint.Publish(new UserCreated
        {
            UserId = Guid.NewGuid(),
            Email = email
        });
    }
}