using EventManagementService.DataAccess;
using EventManagementService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace EventManagementService.IntegrationTests.EventRepositories;

public partial class EventRepositoryTests : IAsyncLifetime
{
    private static readonly List<Event> _events = [
            new Event(id: Guid.NewGuid(),
                title: "event 1",
                description: "Description of event 1",
                totalSeats: 1,
                startAt: new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                endAt: new DateTime(2026, 01, 03, 0, 0, 0, DateTimeKind.Utc)),
            new Event(id: Guid.NewGuid(),
                title: "event 2",
                description: "Description of event 2",
                totalSeats: 1,
                startAt: new DateTime(2026, 02, 04, 0, 0, 0, DateTimeKind.Utc),
                endAt: new DateTime(2026, 02, 05, 0, 0, 0, DateTimeKind.Utc)),
            new Event(id: Guid.NewGuid(),
                title: "event 3",
                description: "Description of event 3",
                totalSeats: 1,
                startAt: new DateTime(2026, 03, 07, 0, 0, 0, DateTimeKind.Utc),
                endAt: new DateTime(2026, 03, 10, 0, 0, 0, DateTimeKind.Utc))];

    private readonly PostgreSqlContainer _postgres;

    public EventRepositoryTests()
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

    public static IEnumerable<object[]> GetEventsWithPagingData()
    {
        yield return new object[] { 1, 10, _events };
        yield return new object[] { 1, 1, new List<Event>() { _events[0] } };
        yield return new object[] { 1, 2, new List<Event>() { _events[0], _events[1] } };
        yield return new object[] { 2, 1, new List<Event>() { _events[1] } };
        yield return new object[] { 3, 2, new List<Event>() { } };
    }
}
