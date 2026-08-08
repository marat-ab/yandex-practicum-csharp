using EventsService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventsService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEventService, EventService>();

        return services;
    }
}
