using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure.Repositories;
using FluentAssertions;

namespace EventManagementService.IntegrationTests.EventRepositories;

public partial class EventRepositoryTests
{
    // Попытка получить событие с несуществующим ID
    [Fact]
    [Trait("Category", "Error")]
    public async Task GetEventByNonExistentId()
    {
        await ResetDatabaseAsync();

        // Arrange
        var eventId = Guid.NewGuid();

        // Act
        var repositoryForCheck = new EventRepository(CreateContext());
        Func<Task> act = async () => await repositoryForCheck.SelectEventByIdAsync(eventId);

        // Assert
        await act.Should().ThrowAsync<EventNotFoundException>()
           .WithMessage($"Can't get event with id = {eventId}. It is absent");
    }

    // Попытка обновить событие с несуществующим ID
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetUpdateEventWithNonExistentId()
    {
        await ResetDatabaseAsync();

        // Arrange
        var eventId = Guid.NewGuid();
        var eventForUpdate = new Event(id: eventId,
                title: "event 1 [updated]",
                description: "Description of event 1 [updated]",
                totalSeats: 1,
                startAt: new DateTime(2026, 05, 01, 0, 0, 0, DateTimeKind.Utc),
                endAt: new DateTime(2026, 05, 03, 0, 0, 0, DateTimeKind.Utc));

        // Act
        var repositoryForCheck = new EventRepository(CreateContext());
        Func<Task> act = async () => await repositoryForCheck.UpdateEventAsync(eventForUpdate);

        // Assert
        await act.Should().ThrowAsync<EventNotFoundException>()
           .WithMessage($"Can't update event with id = {eventId}. It is absent");
    }
}
