using EventManagementService.Domain.Models;
using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Models.Extensions;
using EventManagementService.Repositories;
using EventManagementService.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.IntegrationTests.BookingRepositories;

public partial class BookingRepositoryTests
{
    // Создание брони
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateBooking()
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

        context.Events.Add(eventForAdd);

        var bookingId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 01, 05, 0, 0, 0, DateTimeKind.Utc);

        var newBooking = new Booking(id: bookingId,
            eventId: eventId,
            status: BookingStatus.Pending,
            createdAt: createdAt);

        var expectedBooking = newBooking;
        expectedBooking.Event = eventForAdd;

        // Act
        var repository = new BookingRepository(context);
        await repository.InsertBookingAsync(newBooking);

        // Assert
        await using var verifyContext = CreateContext();
        var bookingFromDb = await verifyContext.Bookings.Include(x => x.Event).FirstOrDefaultAsync();

        bookingFromDb.Should().NotBeNull();

        bookingFromDb.Id.Should().Be(bookingId);
        bookingFromDb.EventId.Should().Be(eventId);
        bookingFromDb.Status.Should().Be(BookingStatus.Pending);
        bookingFromDb.CreatedAt.Should().Be(createdAt);
        bookingFromDb.ProcessedAt.Should().BeNull();

        bookingFromDb.Event!.Id.Should().Be(eventId);
    }

    // Получение брони по Id
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingById()
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

        context.Events.Add(eventForAdd);

        var bookingId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 01, 05, 0, 0, 0, DateTimeKind.Utc);

        var newBooking = new Booking(id: bookingId,
            eventId: eventId,
            status: BookingStatus.Pending,
            createdAt: createdAt);

        var expectedBooking = newBooking;

        context.Bookings.Add(newBooking);
        await context.SaveChangesAsync();

        // Act
        var repository = new BookingRepository(context);
        var bookingFromDb = await repository.SelectBookingByIdAsync(bookingId);

        // Assert
        bookingFromDb.Should().NotBeNull();

        bookingFromDb.Id.Should().Be(bookingId);
        bookingFromDb.EventId.Should().Be(eventId);
        bookingFromDb.Status.Should().Be(BookingStatus.Pending);
        bookingFromDb.CreatedAt.Should().Be(createdAt);
        bookingFromDb.ProcessedAt.Should().BeNull();

        bookingFromDb.Event!.Id.Should().Be(eventId);
    }

    // Получение брони по статусу
    [Fact]
    [Trait("Category", "Success")]
    public async Task GetBookingByStatus()
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

        context.Events.Add(eventForAdd);

        var booking1Id = Guid.NewGuid();
        var booking2Id = Guid.NewGuid();

        var booking1 = new Booking(id: booking1Id,
            eventId: eventId,
            status: BookingStatus.Pending,
            createdAt: DateTime.UtcNow.AddMinutes(-1));

        var createdAt = new DateTime(2026, 01, 05, 0, 0, 0, DateTimeKind.Utc);

        var booking2 = new Booking(id: booking2Id,
            eventId: eventId,
            status: BookingStatus.Confirmed,
            createdAt: createdAt);

        context.Bookings.Add(booking1);
        context.Bookings.Add(booking2);
        await context.SaveChangesAsync();

        var expectedBooking = booking2;

        // Act
        var repository = new BookingRepository(context);
        var bookingFromDb = (await repository.SelectAllBookingByStatusAsync(BookingStatus.Confirmed))[0];

        // Assert
        bookingFromDb.Should().NotBeNull();

        bookingFromDb.Id.Should().Be(booking2Id);
        bookingFromDb.EventId.Should().Be(eventId);
        bookingFromDb.Status.Should().Be(BookingStatus.Confirmed);
        bookingFromDb.CreatedAt.Should().Be(createdAt);
        bookingFromDb.ProcessedAt.Should().BeNull();

        bookingFromDb.Event!.Id.Should().Be(eventId);
    }

    // Обновление брони
    [Fact]
    [Trait("Category", "Success")]
    public async Task UpdateBooking()
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

        context.Events.Add(eventForAdd);

        var bookingId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 01, 05, 0, 0, 0, DateTimeKind.Utc);

        var newBooking = new Booking(id: bookingId,
            eventId: eventId,
            status: BookingStatus.Pending,
            createdAt: createdAt);

        var bookingForUpdate = new Booking(id: bookingId,
            eventId: eventId,
            status: BookingStatus.Confirmed,
            createdAt: createdAt);

        var expectedBooking = bookingForUpdate;

        context.Bookings.Add(newBooking);
        await context.SaveChangesAsync();

        // Act
        var repository = new BookingRepository(context);
        await repository.UpdateBookingAsync(bookingId, bookingForUpdate);

        // Assert
        await using var verifyContext = CreateContext();
        var bookingFromDb = await verifyContext.Bookings.FirstOrDefaultAsync();

        bookingFromDb.Should().NotBeNull();

        bookingFromDb.Id.Should().Be(bookingId);
        bookingFromDb.EventId.Should().Be(eventId);
        bookingFromDb.Status.Should().Be(BookingStatus.Confirmed);
        bookingFromDb.CreatedAt.Should().Be(createdAt);
        bookingFromDb.ProcessedAt.Should().BeNull();
    }
}
