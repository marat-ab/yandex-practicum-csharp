using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Models.Entities;
using EventManagementService.Models.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Tests.Repository;

public partial class BookingRepositoryTests
{
    // Выгрузка всех бронирований из репозитория
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetAllBookingFromRepository()
    {
        // Arrange

        // Act
        var booking = await _bookingRepository.SelectAllBookingAsync();

        // Assert
        booking.Should().BeEquivalentTo(_booking);
    }

    // Выборка бронирования из репозитория по Id
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingFromRepositoryById()
    {
        // Arrange
        var bookingId = _booking[0].Id;
        var expectedBooking = _booking[0];

        // Act
        var booking = (await _bookingRepository.SelectBookingByIdAsync(bookingId)).ToBooking();

        // Assert
        booking.Should().Be(expectedBooking);
    }

    // Выборка бронирования из репозитория по BookingStatus
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingAllFromRepositoryByStatus()
    {
        // Arrange
        var bookingStatus = BookingStatus.Pending;
        var expectedBooking = new List<Booking> { _booking[0], _booking[1] };

        // Act
        var booking = await _bookingRepository.SelectAllBookingByStatusAsync(bookingStatus);

        // Assert
        booking.Should().BeEquivalentTo(expectedBooking);
    }

    // Добавление нового бронирования в репозиторий
    [Fact]
    [Trait("Category", "Success")]
    public async Task InsertBookingToRepository()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var newBooking = new Booking(Id: bookingId, EventId: Guid.NewGuid(),
           Status: BookingStatus.Pending, CreatedAt: new DateTime(2026, 04, 01));

        // Act
        await _bookingRepository.InsertBookingAsync(newBooking.ToBookingEntity());
        var booking = (await _bookingRepository.SelectBookingByIdAsync(bookingId)).ToBooking();

        // Assert
        booking.Should().Be(newBooking);
    }

    // Обновление бронирования в репозиторий
    [Fact]
    [Trait("Category", "Success")]
    public async Task UpdateBookingToRepository()
    {
        // Arrange
        var updatedBookingId = _booking[0].Id;
        var updatedBooking = _booking[0] with
        {
            Status = BookingStatus.Confirmed,
        };

        // Act
        await _bookingRepository.UpdateBookingAsync(updatedBookingId, updatedBooking.ToBookingEntity());
        var booking = (await _bookingRepository.SelectBookingByIdAsync(updatedBookingId)).ToBooking();

        // Assert
        booking.Should().Be(updatedBooking);
    }
}
