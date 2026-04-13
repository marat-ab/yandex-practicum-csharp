using EventManagementService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;

namespace EventManagementService.Tests;

public partial class EventServiceTests
{
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateEvent()
    {
        // Arrange
        var eventForAdd = new Event(
            Id: 0,
            Title: "Some event",
            Description: "Description of event",
            StartAt: new DateTime(2026, 01, 01),
            EndAt: new DateTime(2026, 01, 03));

        var expectedEvent = eventForAdd with { Id = _nextEventIdAfterInit };

        // Act
        var eventWithId = await _eventService.AddEventAsync(eventForAdd);

        // Assert
        eventWithId.Should().Be(expectedEvent);
    }    
}
