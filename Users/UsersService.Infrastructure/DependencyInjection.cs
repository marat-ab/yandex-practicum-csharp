using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Repositories;
using UsersService.Application.Services;
using UsersService.Infrastructure.Repositories;
using UsersService.Infrastructure.Services;

namespace UsersService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();

        services.AddTransient<IJWTService, JWTService>();

        return services;
    }
}
