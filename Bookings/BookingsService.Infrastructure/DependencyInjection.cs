using Bookings.Infrastructure.Repositories;
using BookingsService.Application.Brokers;
using BookingsService.Application.Repositories;
using BookingsService.Infrastructure.Brokers;
using Microsoft.Extensions.DependencyInjection;

namespace BookingsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddSingleton<IBookingProducer, BookingProducer>();

        return services;
    }
}
