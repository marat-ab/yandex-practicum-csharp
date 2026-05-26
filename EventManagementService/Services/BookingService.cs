using EventManagementService.Exceptions;
using EventManagementService.Models;
using EventManagementService.Models.Extensions;
using EventManagementService.Repository;
using EventManagementService.Repository.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace EventManagementService.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _dbc;

    private readonly SemaphoreSlim _bookingSemaphore = new(1, 1);

    public BookingService(AppDbContext dbc)
    {
        _dbc = dbc;
    }

    public async Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken ct = default)
    {
        try
        {
            await _bookingSemaphore.WaitAsync(ct);

            var eventTmp = _dbc.Events
                .Where(x => x.Id == eventId)
                .FirstOrDefault();

            if (eventTmp is null)
                throw new EventNotFoundException(eventId, $"Absent event with id: {eventId}");

            var isReservOk = eventTmp.TryReserveSeats();
            if (isReservOk is false)
                throw new NoAvailableSeatsException(eventId, $"No available seats for event with id {eventId}");
                        
            var newGuid = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;
            var newBooking = new Booking(id: newGuid, 
                eventId: eventId, 
                status: BookingStatus.Pending, 
                createdAt: createdAt);

            await _dbc.Bookings.AddAsync(newBooking, ct);

            await _dbc.SaveChangesAsync(ct);

            return newBooking;
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    public async Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default)
    {
        try
        {
            await _bookingSemaphore.WaitAsync(ct);

            var result = await _dbc.Bookings
                .Where(x => x.Id == bookingId)
                .FirstOrDefaultAsync(ct);

            if (result != null)
            {
                return result;
            }
            else
            {
                throw new BookingNotFoundException(bookingId,
                    $"Can't get booking with id = {bookingId}. It is absent");
            }
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    public async Task<IReadOnlyList<Booking>> GetAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default)
    {
        try
        {
            await _bookingSemaphore.WaitAsync(ct);

            var result = await _dbc.Bookings
                .Where(x => x.Status == status)
                .ToListAsync(ct);

            return result;
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    public async Task UpdateBookingAsync(Guid id, Booking newBooking, CancellationToken ct = default)
    {
        try
        {
            await _bookingSemaphore.WaitAsync(ct);

            var bookingForUpdate = await _dbc.Bookings
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(ct);

            if (bookingForUpdate is null)
            {
                throw new BookingNotFoundException(id,
                   $"Can't get booking with id = {id}. It is absent");
            }

            bookingForUpdate.EventId = newBooking.EventId;
            bookingForUpdate.Status = newBooking.Status;
            bookingForUpdate.CreatedAt = newBooking.CreatedAt;
            bookingForUpdate.ProcessedAt = newBooking.ProcessedAt;


            await _dbc.SaveChangesAsync(ct);
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }
}
