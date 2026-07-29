using BookingsService.Application.HostedServices;
using BookingsService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BookingsService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBookingService, BookingService>();

        services.AddHostedService<BookingHostedService>();

        return services;
    }
}
