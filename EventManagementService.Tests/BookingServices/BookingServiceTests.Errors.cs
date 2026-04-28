using EventManagementService.Exceptions;
using EventManagementService.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Tests.BookingServices;

public partial class BookingServiceTests
{
    // Создание брони для не существующего события
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBookingForNotExistingEvent()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _bookingService.CreateBookingAsync(eventId);

        // Assert
        await act.Should().ThrowAsync<EventNotFoundException>()
           .WithMessage($"Absent event with id: {eventId}");
    }

    // Создание брони для удаленного события
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBookingForDeletedEvent()
    {
        // Arrange
        var eventId = _events[0].Id;

        // Act
        await _eventService.RemoveEventAsync(eventId);

        Func<Task> act = async () => await _bookingService.CreateBookingAsync(eventId);

        // Assert
        await act.Should().ThrowAsync<EventNotFoundException>()
           .WithMessage($"Absent event with id: {eventId}");
    }

    // Получение брони по несуществующему id
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingWithNotExistingId()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _bookingService.GetBookingByIdAsync(bookingId);

        // Assert
        await act.Should().ThrowAsync<BookingNotFoundException>()
           .WithMessage($"Can't get booking with id = {bookingId}. It is absent");
    }
}
