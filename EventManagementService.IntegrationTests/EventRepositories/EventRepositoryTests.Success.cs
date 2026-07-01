using Docker.DotNet.Models;
using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Repositories;
using EventManagementService.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.IntegrationTests.EventRepositories;

public partial class EventRepositoryTests
{
    // Создание события
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateEvent()
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();

        var eventId = Guid.NewGuid();

        var eventForAdd = new Event(id: eventId,
            title: "Some event",
            description: "Description of event",
            totalSeats: 1,
            startAt: new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 01, 03, 0, 0, 0, DateTimeKind.Utc));

        var expectedEvent = eventForAdd;

        var repository = new EventRepository(context);

        // Act
        await repository.InsertEventAsync(eventForAdd);

        // Assert
        await using var verifyContext = CreateContext();
        var saved = await verifyContext.Events.FirstOrDefaultAsync();

        saved.Should().NotBeNull();
        saved.Should().BeEquivalentTo(expectedEvent);
    }

    // Получение события по ID
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventById()
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();

        var eventId = Guid.NewGuid();

        var eventForAdd = new Event(id: eventId,
            title: "Some event",
            description: "Description of event",
            totalSeats: 1,
            startAt: new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 01, 03, 0, 0, 0, DateTimeKind.Utc));

        var expectedEvent = eventForAdd;

        context.Events.Add(eventForAdd);
        await context.SaveChangesAsync();

        // Act
        var repository = new EventRepository(CreateContext());
        var eventWithSpecifiedId = await repository.SelectEventByIdAsync(eventId);

        // Assert
        eventWithSpecifiedId.Should().NotBeNull();
        eventWithSpecifiedId.Should().BeEquivalentTo(expectedEvent);
    }

    // Обновление существующего события
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetUpdateEvent()
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();

        var eventId = Guid.NewGuid();

        var eventForAdd = new Event(id: eventId,
            title: "Some event",
            description: "Description of event",
            totalSeats: 1,
            startAt: new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 01, 03, 0, 0, 0, DateTimeKind.Utc));

        var eventForUpdate = new Event(id: eventId,
                title: "event 1 [updated]",
                description: "Description of event 1 [updated]",
                totalSeats: 1,
                startAt: new DateTime(2026, 05, 01, 0, 0, 0, DateTimeKind.Utc),
                endAt: new DateTime(2026, 05, 03, 0, 0, 0, DateTimeKind.Utc));

        var expectedEvent = eventForUpdate;

        context.Events.Add(eventForAdd);
        await context.SaveChangesAsync();

        // Act
        var repository = new EventRepository(CreateContext());
        await repository.UpdateEventAsync(eventForUpdate);

        // Assert
        var repositoryForCheck = new EventRepository(CreateContext());
        var eventForCheck = await repositoryForCheck.SelectEventByIdAsync(eventId);

        eventForCheck.Should().NotBeNull();
        eventForCheck.Should().BeEquivalentTo(expectedEvent);
    }

    // Удаление существующего события
    [Fact]
    [Trait("Category", "Success")]
    public async Task RemoveEvent()
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();

        var eventId = Guid.NewGuid();

        var eventForAdd = new Event(id: eventId,
            title: "Some event",
            description: "Description of event",
            totalSeats: 1,
            startAt: new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 01, 03, 0, 0, 0, DateTimeKind.Utc));

        var expectedEvent = eventForAdd;

        context.Events.Add(eventForAdd);
        await context.SaveChangesAsync();

        // Act
        var repository = new EventRepository(CreateContext());
        await repository.DeleteEventAsync(eventId);

        // Assert
        var repositoryForCheck = new EventRepository(CreateContext());
        Func<Task> act = async () => await repositoryForCheck.SelectEventByIdAsync(eventId);

        await act.Should().ThrowAsync<EventNotFoundException>()
           .WithMessage($"Can't get event with id = {eventId}. It is absent");
    }

    // Фильтрация по названию
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedTitle_OneEvent()
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();

        var eventId = Guid.NewGuid();
        var title = "event 1";

        var eventForAdd = new Event(id: eventId,
            title: title,
            description: "Description of event",
            totalSeats: 1,
            startAt: new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 01, 03, 0, 0, 0, DateTimeKind.Utc));

        var expectedEvent = eventForAdd;

        context.Events.Add(eventForAdd);
        await context.SaveChangesAsync();

        // Act
        var repository = new EventRepository(CreateContext());
        var eventFromDb = (await repository.SelectAllEventsAsync(title: title)).Events[0];

        // Assert
        eventFromDb.Should().NotBeNull();
        eventFromDb.Should().BeEquivalentTo(expectedEvent);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedTitle_ManyEvents()
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();

        var title = "event";
        List<Event> expectedEvents = _events;

        foreach (var eventExample in _events)
            context.Events.Add(eventExample);

        await context.SaveChangesAsync();

        // Act
        var repository = new EventRepository(CreateContext());
        var events = (await repository.SelectAllEventsAsync(title: title)).Events;

        // Assert
        events.Should().NotBeNull();
        events.Should().BeEquivalentTo(expectedEvents);
    }

    // Фильтрация по датам(startDate, endDate)
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedStartDate()
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();
        
        foreach (var eventExample in _events)
            context.Events.Add(eventExample);

        await context.SaveChangesAsync();

        var startDate = new DateTime(2026, 02, 01, 0, 0, 0, DateTimeKind.Utc);
        List<Event> expectedEvents = [_events[1], _events[2]];

        // Act
        var repository = new EventRepository(CreateContext());
        var events = (await repository.SelectAllEventsAsync(from: startDate)).Events;

        // Assert
        events.Should().NotBeNull();
        events.Should().BeEquivalentTo(expectedEvents);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedEndDate()
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();

        foreach (var eventExample in _events)
            context.Events.Add(eventExample);

        await context.SaveChangesAsync();

        var endDate = new DateTime(2026, 02, 05, 0, 0, 0, DateTimeKind.Utc);
        List<Event> expectedEvents = [_events[0], _events[1]];

        // Act
        var repository = new EventRepository(CreateContext());
        var events = (await repository.SelectAllEventsAsync(to: endDate)).Events;

        // Assert
        events.Should().NotBeNull();
        events.Should().BeEquivalentTo(expectedEvents);
    }

    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedStartAndEndDate()
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();

        foreach (var eventExample in _events)
            context.Events.Add(eventExample);

        await context.SaveChangesAsync();

        var startDate = new DateTime(2026, 02, 01, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 02, 20, 0, 0, 0, DateTimeKind.Utc);
        List<Event> expectedEvents = [_events[1]];

        // Act
        var repository = new EventRepository(CreateContext());
        var events = (await repository.SelectAllEventsAsync(from: startDate, to: endDate)).Events;

        // Assert
        events.Should().NotBeNull();
        events.Should().BeEquivalentTo(expectedEvents);
    }

    // Пагинация событий
    [Theory]
    [Trait("Category", "Success")]
    [MemberData(nameof(GetEventsWithPagingData))]
    public async Task GetEventsWithPaging(int pageNumber, int pageSize, List<Event> expectedEvents)
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();

        foreach (var eventExample in _events)
            context.Events.Add(eventExample);

        await context.SaveChangesAsync();

        var expectedPaginatedResult = new PaginatedResult()
        {
            TotalEventsCount = _events.Count,
            Events = expectedEvents,
            PageNumber = pageNumber,
            EventsCountOnPage = expectedEvents.Count
        };

        // Act
        var repository = new EventRepository(CreateContext());
        var result = await repository.SelectAllEventsAsync(pageNumber: pageNumber, pageSize: pageSize);

        // Assert
        result.Should().NotBeNull();
        result.TotalEventsCount.Should().Be(expectedPaginatedResult.TotalEventsCount);
        result.PageNumber.Should().Be(expectedPaginatedResult.PageNumber);
        result.EventsCountOnPage.Should().Be(expectedPaginatedResult.EventsCountOnPage);
    }

    // Комбинированная фильтрация
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetEventsWithSpecifiedFilters()
    {
        await ResetDatabaseAsync();

        // Arrange
        await using var context = CreateContext();

        foreach (var eventExample in _events)
            context.Events.Add(eventExample);

        await context.SaveChangesAsync();
                
        var title = "event";
        var startDate = new DateTime(2026, 02, 01, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 02, 20, 0, 0, 0, DateTimeKind.Utc);
        List<Event> expectedEvents = [_events[1]];

        // Act
        var repository = new EventRepository(CreateContext());
        var events = (await repository.SelectAllEventsAsync(title: title, from: startDate, to: endDate)).Events;

        // Assert
        events.Should().NotBeNull();
        events.Should().BeEquivalentTo(expectedEvents);
    }
}
