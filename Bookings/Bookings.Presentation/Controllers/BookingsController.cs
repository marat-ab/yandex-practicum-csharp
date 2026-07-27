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
[Route("/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("{id:Guid}")]
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

    [HttpDelete("{id:Guid}")]
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
}
