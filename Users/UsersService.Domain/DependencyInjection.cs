using Microsoft.Extensions.DependencyInjection;
using UsersService.Domain.Services;

namespace UsersService.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddTransient<IEncryptionService, EncryptionService>();

        return services;
    }
}
