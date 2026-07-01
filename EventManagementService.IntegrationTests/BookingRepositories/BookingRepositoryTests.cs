using EventManagementService.DataAccess;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace EventManagementService.IntegrationTests.BookingRepositories;

public partial class BookingRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;

    public BookingRepositoryTests()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("eventapi")
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
        await context.Database.MigrateAsync();
    }
}
