using EventManagementService.Models.Dto;
using EventManagementService.Models.Extensions;
using EventManagementService.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementService.Controllers;

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
        var bookingItem = await _bookingService.GetBookingByIdAsync(id);

        var result = bookingItem.ToBookingResponseDto();

        return Ok(result);
    }
}
