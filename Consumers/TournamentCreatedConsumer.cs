using MassTransit;
using Microsoft.Extensions.Logging;

public class TournamentCreatedConsumer : IConsumer<TournamentCreated>
{
    private readonly ILogger<TournamentCreatedConsumer> _logger;

    public TournamentCreatedConsumer(ILogger<TournamentCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<TournamentCreated> context)
    {
        _logger.LogInformation("Tournament created: {TournamentId}", context.Message.TournamentId);
        return Task.CompletedTask;
    }
}
