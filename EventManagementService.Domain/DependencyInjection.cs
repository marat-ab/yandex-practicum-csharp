using EventManagementService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddTransient<IEncryptionService, EncryptionService>();

        return services;
    }
}
