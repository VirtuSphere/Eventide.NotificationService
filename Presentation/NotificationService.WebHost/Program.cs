using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NotificationService.Domain.Repositories.Abstractions.Base;
using NotificationService.Domain.Repositories.Abstractions.Repositories;
using NotificationService.Infrastructure.EntityFramework;
using NotificationService.Infrastructure.EntityFramework.RepositoriesEF;
using NotificationService.WebHost.Helpers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString(nameof(ApplicationDbContext));

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string for NotificationServiceDbContext is not configured.");
}

builder.Services.AddNpgsql<ApplicationDbContext>(connectionString, options =>
{
    options.MigrationsAssembly("NotificationService.Infrastructure.EntityFramework");

});

builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Notification service API",
            Description = "API for creating, viewing, storing, modifying, and deleting notifications."
        });
    });

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseNpgsql(connectionString);
    });

builder.Services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
builder.Services.AddScoped<IAdministratorRepository, EfAdministratorRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IRelatedEntityRepository, EfRelatedEntityRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.MigrateDatabase<ApplicationDbContext>();

app.Run();
