using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Tests.EventServices;

public partial class EventServiceTests
{
    // Попытка получить событие с несуществующим ID
    [Fact]
    [Trait("Category", "Error")]
    public async Task GetEventByNonExistentId()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var eventId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await eventService.GetEventByIdAsync(eventId);

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
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var eventId = Guid.NewGuid();
        var eventForUpdate = new Event(id: eventId,
                title: "event 1 [updated]",
                description: "Description of event 1 [updated]",
                totalSeats: 1,
                startAt: new DateTime(2026, 05, 01),
                endAt: new DateTime(2026, 05, 03));

        // Act
        Func<Task> act = async () => await eventService.UpdateEventAsync(eventForUpdate);

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
        using var scope = _serviceProvider.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

        var eventForAdd = new Event(id: Guid.Empty,
                title: title,
                description: "Description",
                totalSeats: 1,
                startAt: startAt,
                endAt: endAt);

        // Act
        Func<Task> act = async () => await eventService.AddEventAsync(eventForAdd);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
           .WithMessage(errorMsg);
    }    
}
