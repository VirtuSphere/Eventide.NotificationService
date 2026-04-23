using Eventide.NotificationService.Domain.Interfaces;
using Eventide.NotificationService.Infrastructure.Data;
using Eventide.NotificationService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eventide.NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("NotificationDb")));

        services.AddScoped<INotificationRepository, NotificationRepository>();
        return services;
    }
}