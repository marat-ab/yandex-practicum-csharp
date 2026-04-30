using EventManagementService.Exceptions;
using EventManagementService.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Tests.EventServices;

public partial class EventServiceTests
{
    // Создание события
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateEvent()
    {
        // Arrange

        var eventId = Guid.NewGuid();

        var eventForAdd = new Event(
            Id: eventId,
            Title: "Some event",
            Description: "Description of event",
            StartAt: new DateTime(2026, 01, 01),
            EndAt: new DateTime(2026, 01, 03));

        var expectedEvent = eventForAdd with { Id = eventId };

        // Act
        var eventWithId = await _eventService.AddEventAsync(eventForAdd);

        // Assert
        eventWithId.Should().Be(expectedEvent);
    }

    // Получение всех событий
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetAllEvents()
    {
        // Arrange
        var expectedEvents = _events;

        // Act
        var events = (await _eventService.GetAllEventsAsync()).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }

    // Получение события по ID
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventById()
    {
        // Arrange
        var eventId = _events[0].Id;
        var expectedEvent = _events[0];

        // Act
        var eventWithSpecifiedId = await _eventService.GetEventByIdAsync(eventId);

        // Assert
        eventWithSpecifiedId.Should().Be(expectedEvent);
    }

    // Обновление существующего события
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetUpdateEvent()
    {
        // Arrange
        var eventId = _events[0].Id;
        var eventForUpdate = new Event(Id: eventId,
                Title: "event 1 [updated]",
                Description: "Description of event 1 [updated]",
                StartAt: new DateTime(2026, 05, 01),
                EndAt: new DateTime(2026, 05, 03));

        // Act
        await _eventService.UpdateEventAsync(eventForUpdate);
        var eventFromService = await _eventService.GetEventByIdAsync(eventId);

        // Assert
        eventFromService.Should().Be(eventForUpdate);
    }

    // Удаление существующего события
    [Fact]
    [Trait("Category", "Success")]
    public async Task RemoveEvent()
    {
        // Arrange
        var eventId = _events[0].Id;
        var eventForDelete = await _eventService.GetEventByIdAsync(eventId);

        // Act
        await _eventService.RemoveEventAsync(eventId);

        Func<Task> act = async () => await _eventService.GetEventByIdAsync(eventId);

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
        var title = "event 1";
        List<Event> expectedEvents = [_events[0]];

        // Act
        var events = (await _eventService.GetAllEventsAsync(title: title)).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedTitle_ManyEvents()
    {
        // Arrange
        var title = "event";
        List<Event> expectedEvents = _events;

        // Act
        var events = (await _eventService.GetAllEventsAsync(title: title)).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }

    // Фильтрация по датам(startDate, endDate)
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedStartDate()
    {
        // Arrange
        var startDate = new DateTime(2026, 02, 01);
        List<Event> expectedEvents = [_events[1], _events[2]];

        // Act
        var events = (await _eventService.GetAllEventsAsync(from: startDate)).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedEndDate()
    {
        // Arrange
        var endDate = new DateTime(2026, 02, 05);
        List<Event> expectedEvents = [_events[0], _events[1]];

        // Act
        var events = (await _eventService.GetAllEventsAsync(to: endDate)).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedStartAndEndDate()
    {
        // Arrange
        var startDate = new DateTime(2026, 02, 01);
        var endDate = new DateTime(2026, 02, 20);
        List<Event> expectedEvents = [_events[1]];

        // Act
        var events = (await _eventService.GetAllEventsAsync(from: startDate, to: endDate)).Events;

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
        var expectedPaginatedResult = new PaginatedResult()
        {
            TotalEventsCount = _events.Count,
            Events = expectedEvents,
            PageNumber = pageNumber,
            EventsCountOnPage = expectedEvents.Count
        };

        // Act
        var result = await _eventService.GetAllEventsAsync(pageNumber: pageNumber, pageSize: pageSize);

        // Assert
        result.Should().BeEquivalentTo(expectedPaginatedResult);
    }

    // Комбинированная фильтрация
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedFilters()
    {
        // Arrange
        var title = "event";
        var startDate = new DateTime(2026, 02, 01);
        var endDate = new DateTime(2026, 02, 20);
        List<Event> expectedEvents = [_events[1]];

        // Act
        var events = (await _eventService.GetAllEventsAsync(title: title, from: startDate, to: endDate)).Events;

        // Assert
        events.Should().BeEquivalentTo(expectedEvents);
    }
}
