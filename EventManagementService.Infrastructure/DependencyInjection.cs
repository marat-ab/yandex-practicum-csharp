using EventManagementService.Application.HostedServices;
using EventManagementService.Application.Repositories;
using EventManagementService.Application.Services;
using EventManagementService.Infrastructure.Repositories;
using EventManagementService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddTransient<IUserRepository, UserRepository>();

        services.AddTransient<IJWTService, JWTService>();

        return services;
    }
}
