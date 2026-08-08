using EventsService.Application.Repositories;
using EventsService.Infrastructure.HostedServices;
using EventsService.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IEventRepository, EventRepository>();

        services.AddHostedService<BookingConfirmedHostedService>();

        return services;
    }
}
