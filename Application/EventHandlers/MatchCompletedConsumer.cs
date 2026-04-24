using Eventide.MatchService.Contracts.Events;
using Eventide.NotificationService.Application.Commands.SendNotification;
using Eventide.NotificationService.Domain.Enums;
using MassTransit;
using MediatR;

namespace Eventide.NotificationService.Application.EventHandlers;

public class MatchCompletedConsumer : IConsumer<MatchCompletedEvent>
{
    private readonly IMediator _mediator;

    public MatchCompletedConsumer(IMediator mediator) => _mediator = mediator;

    public async Task Consume(ConsumeContext<MatchCompletedEvent> context)
    {
        var msg = context.Message;
        
        await _mediator.Send(new SendNotificationCommand
        {
            RecipientUserId = msg.WinnerId,
            Title = "Match Won!",
            Body = $"You won the match with score {msg.WinnerScore}:{msg.LoserScore}",
            Type = NotificationType.MatchResult
        });
        
        await _mediator.Send(new SendNotificationCommand
        {
            RecipientUserId = msg.LoserId,
            Title = "Match Lost",
            Body = $"You lost the match with score {msg.LoserScore}:{msg.WinnerScore}",
            Type = NotificationType.MatchResult
        });
    }
}