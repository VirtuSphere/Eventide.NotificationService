using MassTransit;
using NotificationService.Consumers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<TestPublisherBackgroundService>();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreatedConsumer>();
    x.AddConsumer<TournamentCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("user-created-queue", e =>
        {
            e.ConfigureConsumer<UserCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("tournament-created-queue", e =>
        {
            e.ConfigureConsumer<TournamentCreatedConsumer>(context);
        });
    });
});

var app = builder.Build();
await app.RunAsync();
