using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Repositories;
using UsersService.Application.Services;
using UsersService.Domain.Services;
using UsersService.Infrastructure.DataAccess;
using UsersService.Infrastructure.Models;
using UsersService.Infrastructure.Repositories;
using UsersService.Infrastructure.Services;

namespace UsersService.Tests.UserServices;

public partial class UserServiceTests : IAsyncLifetime
{
    private readonly ServiceProvider _serviceProvider;

    public UserServiceTests()
    {
        var dbName = Guid.NewGuid().ToString();

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<IUserService, UserService>();

        services.Configure<JWTSettings>(options =>
        {
            options.Key = "1234567890123456789012345678901234567890";
            options.Issuer = "TestIssuer";
            options.Audience = "TestAudience";
            options.ExpirationTimeMinutes = 60;
        });

        _serviceProvider = services.BuildServiceProvider();
    }


    // IAsyncLifetime
    public async Task InitializeAsync()
    {

    }

    public async Task DisposeAsync()
    {

    }
}
