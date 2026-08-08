using System;
using System.Collections.Generic;
using System.Text;

namespace BookingsService.Application.Brokers;

public interface IBookingProducer
{
    Task BookingConfirmedAsync(Guid eventId);
}
