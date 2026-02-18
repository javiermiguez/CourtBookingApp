using Bookings.Application;
using Bookings.Application.DTOs.Requests;
using Bookings.Application.DTOs.Responses;
using Bookings.Common;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingAppService _bookingService;

    public BookingsController(IBookingAppService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateBooking([FromBody] CreateBookingRequest request)
    {
        var userId = Guid.NewGuid(); // Get userId from JWT token (to be implemented)
        var result = await _bookingService.CreateBookingAsync(userId, request);

        if (!result.IsSuccess)
            return ErrorToActionResult(result.Error);

        return CreatedAtAction(nameof(GetBooking), new { id = result.Value }, result.Value);
    }

    [HttpPost("{id}/players")]
    public async Task<ActionResult> AddPlayer(Guid id, [FromBody] AddPlayerRequest request)
    {
        var result = await _bookingService.AddPlayerAsync(id, request);

        if (!result.IsSuccess)
            return ErrorToActionResult(result.Error);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponse>> GetBooking(Guid id)
    {
        var booking = await _bookingService.GetBookingResponseAsync(id);

        if (!booking.IsSuccess)
            return NotFound();

        return Ok(booking);
    }

    private ActionResult ErrorToActionResult(Error error)
    {
        var detail = error.Message;
        var instance = HttpContext.Request.Path;
        Dictionary<string, object?> extensions = new() { ["errorCode"] = error.Code };

        var result = error.Code switch
        {
            // 404 - Not Found
            "BookingNotFound" => NotFound(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                Status = 404,
                Title = "Not Found",
                Detail = detail,
                Instance = instance,
                Extensions = extensions
            }),

            // 409 - Conflict (bussiness logic)
            "Booking.OnlyMatchmakingCanAddPlayers" or
            "Booking.BookingNotWaiting" or
            "Booking.InvalidPlayerRank" or
            "Booking.PlayerAlreadyInBooking" => Conflict(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                Status = 409,
                Title = "Conflict",
                Detail = detail,
                Instance = instance,
                Extensions = extensions
            }),

            // 400 - Bad Request (validation)
            "InvalidModality" or
            "InvalidGameType" or
            "InvalidPlayerRank" or
            "InvalidPeriod" => BadRequest(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Status = 400,
                Title = "Bad Request",
                Detail = detail,
                Instance = instance,
                Extensions = extensions
            }),

            // 500 - Internal Error
            _ => StatusCode(500, new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Status = 500,
                Title = "Internal Server Error",
                Detail = detail,
                Instance = instance,
                Extensions = extensions
            })
        };

        return result;
    }
}
