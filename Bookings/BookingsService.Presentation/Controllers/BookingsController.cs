using Bookings.Application.Models.Dto;
using Bookings.Application.Services;
using Bookings.Domain.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Bookings.Application.Models.Extensions;

namespace Bookings.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("/")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("bookings/{id:Guid}")]
    public async Task<ActionResult<BookingResponseDto>> GetBookingById(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return BadRequest("User id not found");

        if (Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            var bookingItem = await _bookingService.GetBookingByIdAsync(id, userId);

            var result = bookingItem.ToBookingResponseDto();

            return Ok(result);
        }
        else
        {
            return BadRequest("Bad user id");
        }
    }

    [HttpDelete("bookings/{id:Guid}")]
    public async Task<ActionResult> DeleteBookingById(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userRoleClaim = User.FindFirst(ClaimTypes.Role);

        if (userIdClaim == null)
            return BadRequest("User id not found");

        if (userRoleClaim == null)
            return BadRequest("Role not found");

        if (Guid.TryParse(userIdClaim.Value, out Guid userId)
            && Enum.TryParse(userRoleClaim.Value, out Role role))
        {
            await _bookingService.CancelBookingAsync(id, userId, role);

            return NoContent();
        }
        else
        {
            return BadRequest("Bad user id or role");
        }
    }

    [HttpPost("events/{eventId:Guid}/book")]
    public async Task<ActionResult<BookingResponseDto>> BookingEvent(Guid eventId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return BadRequest("User id not found");

        if (Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            var bookingItem = await _bookingService.CreateBookingAsync(eventId, userId);

            var url = $"/bookings/{bookingItem.Id}";
            Response.Headers.Location = url;

            var result = bookingItem.ToBookingResponseDto();

            return Accepted(result);
        }
        else
        {
            return BadRequest("Bad user id");
        }
    }
}
