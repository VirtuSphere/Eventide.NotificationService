using Eventide.NotificationService.Application;
using Eventide.NotificationService.Infrastructure;
using Eventide.NotificationService.Application.EventHandlers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TournamentCreatedConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("eventide");
            h.Password("eventide_pass");
        });
        
        cfg.ReceiveEndpoint("tournament-created-queue", e =>
        {
            e.ConfigureConsumer<TournamentCreatedConsumer>(context);
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();