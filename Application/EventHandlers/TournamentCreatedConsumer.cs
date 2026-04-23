using Eventide.NotificationService.Application.Commands.SendNotification;
using Eventide.NotificationService.Domain.Enums;
using Eventide.TournamentService.Contracts.Events;
using MassTransit;
using MediatR;

namespace Eventide.NotificationService.Application.EventHandlers;

public class TournamentCreatedConsumer : IConsumer<TournamentCreatedEvent>
{
    private readonly IMediator _mediator;

    public TournamentCreatedConsumer(IMediator mediator) => _mediator = mediator;

    public async Task Consume(ConsumeContext<TournamentCreatedEvent> context)
    {
        var message = context.Message;
        
        await _mediator.Send(new SendNotificationCommand
        {
            RecipientUserId = message.OrganizerId,
            Title = "Tournament Created!",
            Body = $"Your tournament '{message.TournamentName}' has been created",
            Type = NotificationType.SystemMessage
        });
    }
}