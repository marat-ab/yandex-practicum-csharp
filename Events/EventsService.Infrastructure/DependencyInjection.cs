using EventsService.Application.Repositories;
using EventsService.Application.Services;
using EventsService.Infrastructure.Caches;
using EventsService.Infrastructure.HostedServices;
using EventsService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IEventRepository, EventRepository>();

        services.AddScoped<ICacheService, RedisCacheService>();

        services.AddHostedService<BookingConfirmedHostedService>();

        return services;
    }
}
