using EventManagementService.Application.Services;
using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagementService.Tests.EventServices;

public partial class EventServiceTests
{
    // Создание события
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateEvent()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var eventId = Guid.NewGuid();

        var eventForAdd = new Event(id: eventId,
            title: "Some event",
            description: "Description of event",
            totalSeats: 1,
            startAt: new DateTime(2026, 01, 01),
            endAt: new DateTime(2026, 01, 03));

        var expectedEvent = eventForAdd;

        // Act
        await eventService.AddEventAsync(eventForAdd);
        var eventFromStore = await eventService.GetEventByIdAsync(eventId);

        // Assert
        eventFromStore.Should().BeEquivalentTo(expectedEvent);
    }

    // Получение всех событий
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetAllEvents()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var expectedEvents = _events;

        // Act
        var events = (await eventService.GetAllEventsAsync()).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }

    // Получение события по ID
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventById()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var eventId = _events[0].Id;
        var expectedEvent = _events[0];

        // Act
        var eventWithSpecifiedId = await eventService.GetEventByIdAsync(eventId);

        // Assert
        eventWithSpecifiedId.Should().BeEquivalentTo(expectedEvent);
    }

    // Обновление существующего события
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetUpdateEvent()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var eventId = _events[0].Id;
        var eventForUpdate = new Event(id: eventId,
                title: "event 1 [updated]",
                description: "Description of event 1 [updated]",
                totalSeats: 1,
                startAt: new DateTime(2026, 05, 01),
                endAt: new DateTime(2026, 05, 03));

        // Act
        await eventService.UpdateEventAsync(eventForUpdate);
        var eventFromService = await eventService.GetEventByIdAsync(eventId);

        // Assert
        eventFromService.Should().BeEquivalentTo(eventForUpdate);
    }

    // Удаление существующего события
    [Fact]
    [Trait("Category", "Success")]
    public async Task RemoveEvent()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var eventId = _events[0].Id;
        var eventForDelete = await eventService.GetEventByIdAsync(eventId);

        // Act
        await eventService.RemoveEventAsync(eventId);

        Func<Task> act = async () => await eventService.GetEventByIdAsync(eventId);

        // Assert
        await act.Should().ThrowAsync<EventNotFoundException>()
           .WithMessage($"Can't get event with id = {eventId}. It is absent");
    }

    // Фильтрация по названию
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedTitle_OneEvent()
    {
        // Arrange        
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var title = "event 1";
        List<Event> expectedEvents = [_events[0]];

        // Act
        var events = (await eventService.GetAllEventsAsync(title: title)).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedTitle_ManyEvents()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var title = "event";
        List<Event> expectedEvents = _events;

        // Act
        var events = (await eventService.GetAllEventsAsync(title: title)).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }

    // Фильтрация по датам(startDate, endDate)
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedStartDate()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var startDate = new DateTime(2026, 02, 01);
        List<Event> expectedEvents = [_events[1], _events[2]];

        // Act
        var events = (await eventService.GetAllEventsAsync(from: startDate)).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedEndDate()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var endDate = new DateTime(2026, 02, 05);
        List<Event> expectedEvents = [_events[0], _events[1]];

        // Act
        var events = (await eventService.GetAllEventsAsync(to: endDate)).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedStartAndEndDate()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var startDate = new DateTime(2026, 02, 01);
        var endDate = new DateTime(2026, 02, 20);
        List<Event> expectedEvents = [_events[1]];

        // Act
        var events = (await eventService.GetAllEventsAsync(from: startDate, to: endDate)).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }

    // Пагинация событий
    [Theory]
    [Trait("Category", "Success")]
    [MemberData(nameof(GetEventsWithPagingData))]
    public async Task GetEventsWithPaging(int pageNumber, int pageSize, List<Event> expectedEvents)
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var expectedPaginatedResult = new PaginatedResult()
        {
            TotalEventsCount = _events.Count,
            Events = expectedEvents,
            PageNumber = pageNumber,
            EventsCountOnPage = expectedEvents.Count
        };

        // Act
        var result = await eventService.GetAllEventsAsync(pageNumber: pageNumber, pageSize: pageSize);

        // Assert
        result.Should().BeEquivalentTo(expectedPaginatedResult);
    }

    // Комбинированная фильтрация
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedFilters()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var title = "event";
        var startDate = new DateTime(2026, 02, 01);
        var endDate = new DateTime(2026, 02, 20);
        List<Event> expectedEvents = [_events[1]];

        // Act
        var events = (await eventService.GetAllEventsAsync(title: title, from: startDate, to: endDate)).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }
}
