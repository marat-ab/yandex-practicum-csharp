using EventManagementService.Application.Repositories;
using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EventManagementService.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _dbc;

    private static readonly SemaphoreSlim _bookingSemaphore = new(1, 1);

    public BookingRepository(AppDbContext dbc)
    {
        _dbc = dbc;
    }

    public async Task<Booking> InsertBookingAsync(Booking newBooking, CancellationToken ct = default)
    {
        try
        {
            await _bookingSemaphore.WaitAsync(ct);

            await _dbc.Bookings.AddAsync(newBooking, ct);

            await _dbc.SaveChangesAsync(ct);

            return newBooking;
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    public async Task<Booking> SelectBookingByIdAsync(Guid bookingId, Guid userId, CancellationToken ct = default)
    {
        try
        {
            await _bookingSemaphore.WaitAsync(ct);

            var result = await _dbc.Bookings
                .Where(x => x.Id == bookingId)
                .Where(x => x.UserId == userId)
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

    public async Task<IReadOnlyList<Booking>> SelectAllActiveBookingForUserAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            await _bookingSemaphore.WaitAsync(ct);

            var result = await _dbc.Bookings
                .Where(x => x.UserId == userId)
                .Where(x => x.Status == BookingStatus.Confirmed || x.Status == BookingStatus.Pending )
                .ToListAsync(ct);

            return result;
        }
        finally
        {
            _bookingSemaphore.Release();
        }
    }

    public async Task<IReadOnlyList<Booking>> SelectAllBookingByStatusAsync(BookingStatus status, CancellationToken ct = default)
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

    public async Task UpdateBookingAsync(Guid bookingId, Booking newBooking, CancellationToken ct = default)
    {
        try
        {
            await _bookingSemaphore.WaitAsync(ct);

            var bookingForUpdate = await _dbc.Bookings
                .Where(x => x.Id == bookingId)
                .FirstOrDefaultAsync(ct);

            if (bookingForUpdate is null)
            {
                throw new BookingNotFoundException(bookingId,
                   $"Can't get booking with id = {bookingId}. It is absent");
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
