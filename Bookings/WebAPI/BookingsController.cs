using Bookings.Application;
using Bookings.Application.DTOs.Requests;
using Bookings.Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingAppService _bookingService;

    public BookingsController(BookingAppService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateBooking([FromBody] CreateBookingRequest request)
    {
        var userId = Guid.NewGuid(); // Get userId from JWT token (to be implemented)
        var bookingId = await _bookingService.CreateBookingAsync(userId, request);
        return CreatedAtAction(nameof(GetBooking), new { id = bookingId }, bookingId);
    }

    [HttpPost("{id}/players")]
    public async Task<ActionResult> AddPlayer(Guid id, [FromBody] AddPlayerRequest request)
    {
        var success = await _bookingService.AddPlayerAsync(id, request);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponse>> GetBooking(Guid id)
    {
        var booking = await _bookingService.GetBookingResponseAsync(id);

        if (booking == null)
            return NotFound();

        return Ok(booking);
    }
}
