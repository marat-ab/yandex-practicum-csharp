using EventManagementService.Exceptions;
using EventManagementService.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Tests.EventService;

public partial class EventServiceTests
{
    // Попытка получить событие с несуществующим ID
    [Fact]
    [Trait("Category", "Error")]
    public async Task GetEventByNonExistentId()
    {
        // Arrange
        var eventId = 5;

        // Act
        Func<Task> act = async () => await _eventService.GetEventByIdAsync(eventId);

        // Assert
        await act.Should().ThrowAsync<EventNotFoundException>()
           .WithMessage($"Can't get event with id = {eventId}. It is absent");
    }

    // Попытка обновить событие с несуществующим ID
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetUpdateEventWithNonExistentId()
    {
        // Arrange
        var eventId = 5;
        var eventForUpdate = new Event(Id: eventId,
                Title: "event 1 [updated]",
                Description: "Description of event 1 [updated]",
                StartAt: new DateTime(2026, 05, 01),
                EndAt: new DateTime(2026, 05, 03));

        // Act
        Func<Task> act = async () => await _eventService.UpdateEventAsync(eventForUpdate);

        // Assert
        await act.Should().ThrowAsync<EventNotFoundException>()
           .WithMessage($"Can't update event with id = {eventId}. It is absent");
    }

    // Создание события с некорректными данными(если валидация в сервисе)
    // Обновление события с некорректными датами(EndAt раньше StartAt)
    [Theory]
    [Trait("Category", "Success")]
    [MemberData(nameof(GetBadEventParams))]
    public async Task AddEventWithBadData(string title, DateTime startAt, DateTime endAt, string errorMsg)
    {
        // Arrange
        var eventForAdd = new Event(Id: 0,
                Title: title,
                Description: "Description",
                StartAt: startAt,
                EndAt: endAt);

        // Act
        Func<Task> act = async () => await _eventService.AddEventAsync(eventForAdd);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
           .WithMessage(errorMsg);
    }    
}
