using Bookings.Application.Repositories;
using Bookings.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Bookings.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IBookingRepository, BookingRepository>();

        return services;
    }
}
