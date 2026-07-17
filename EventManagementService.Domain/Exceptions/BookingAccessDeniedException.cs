using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Domain.Exceptions;

public sealed class BookingAccessDeniedException : Exception
{
    public Guid UserId { get; init; }

    public BookingAccessDeniedException()
        : base(message: "Booking access denied error (without user id)")
    {
    }

    public BookingAccessDeniedException(Guid userId, string message)
        : base(message: message)
    {
        UserId = userId;
    }
}
