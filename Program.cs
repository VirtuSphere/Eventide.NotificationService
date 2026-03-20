using MassTransit;
using NotificationService.Consumers;
using NotificationService.Publishers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ⚡ Настройка MassTransit один раз
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreatedConsumer>();

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
    });
});

// Publisher через DI
builder.Services.AddScoped<UserCreatedPublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ⚡ Отправка тестового сообщения после старта приложения
var scope = app.Services.CreateScope();
var publisher = scope.ServiceProvider.GetRequiredService<UserCreatedPublisher>();
await publisher.PublishUserCreated("test@example.com");

app.Run();