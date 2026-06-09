using EventManagementService.DataAccess;
using EventManagementService.Models;
using EventManagementService.Repositories;
using EventManagementService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Testcontainers.PostgreSql;

namespace EventManagementService.IntegrationTests.EventRepository;

public partial class EventRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;

    public EventRepositoryTests()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine")
            .Build();
    }

    
    // IAsyncLifetime
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    // Private 
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.Migrate();
        return context;
    }

    private async Task ResetDatabaseAsync()
    {
        NpgsqlConnection.ClearAllPools();

        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
