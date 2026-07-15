using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Domain.Exceptions;

public sealed class BookingCancelAccessDeniedException : Exception
{
    public Guid UserId { get; init; }

    public BookingCancelAccessDeniedException()
        : base(message: "Booking cancel access denied by role error (without user id)")
    {
    }

    public BookingCancelAccessDeniedException(Guid userId, string message)
        : base(message: message)
    {
        UserId = userId;
    }
}
