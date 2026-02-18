using Bookings.Application;
using Bookings.Application.DTOs.Requests;
using Bookings.Application.DTOs.Responses;
using Bookings.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Bookings.Tests.Unit.WebAPI;

public class BookingsControllerTests
{
    private readonly Mock<IBookingAppService> _mockService;
    private readonly BookingsController _controller;

    public BookingsControllerTests()
    {
        _mockService = new Mock<IBookingAppService>();
        _controller = new BookingsController(_mockService.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Path = "/api/bookings" } }
        };
    }

    [Fact]
    public async Task CreateBooking_ReturnsCreated()
    {
        var request = new CreateBookingRequest(Guid.NewGuid(), DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2), "Matchmaking", "Doubles", "Intermediate", 25);
        var bookingId = Guid.NewGuid();

        _mockService.Setup(x => x.CreateBookingAsync(It.IsAny<Guid>(), request))
            .ReturnsAsync(Result.Success(bookingId));

        var result = await _controller.CreateBooking(request);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(bookingId, created.Value);
    }

    [Fact]
    public async Task CreateBooking_WhenFails_ReturnsError()
    {
        var request = new CreateBookingRequest(Guid.NewGuid(), DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(2), "Matchmaking", "Doubles", "Intermediate", 25);
        var error = new Error("InvalidPeriod", "End time must be after start time");

        _mockService.Setup(x => x.CreateBookingAsync(It.IsAny<Guid>(), request))
            .ReturnsAsync(Result.Failure<Guid>(error));

        var result = await _controller.CreateBooking(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var problem = Assert.IsType<ProblemDetails>(badRequest.Value);
        Assert.Equal(400, problem.Status);
        Assert.Equal("InvalidPeriod", problem.Extensions["errorCode"]);
    }

    [Fact]
    public async Task AddPlayer_ReturnsNoContent()
    {
        var bookingId = Guid.NewGuid();
        var request = new AddPlayerRequest(Guid.NewGuid(), "Intermediate");

        _mockService.Setup(x => x.AddPlayerAsync(bookingId, request))
            .ReturnsAsync(Result.Success(true));

        var result = await _controller.AddPlayer(bookingId, request);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task AddPlayer_WhenNotFound_Returns404()
    {
        var bookingId = Guid.NewGuid();
        var request = new AddPlayerRequest(Guid.NewGuid(), "Intermediate");
        var error = new Error("BookingNotFound", "Booking not found");

        _mockService.Setup(x => x.AddPlayerAsync(bookingId, request))
            .ReturnsAsync(Result.Failure<bool>(error));

        var result = await _controller.AddPlayer(bookingId, request);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var problem = Assert.IsType<ProblemDetails>(notFound.Value);
        Assert.Equal(404, problem.Status);
        Assert.Equal("BookingNotFound", problem.Extensions["errorCode"]);
    }

    [Fact]
    public async Task AddPlayer_WhenBusinessRuleFails_Returns409()
    {
        var bookingId = Guid.NewGuid();
        var request = new AddPlayerRequest(Guid.NewGuid(), "Intermediate");
        var error = new Error("Booking.InvalidPlayerRank", "Invalid rank");

        _mockService.Setup(x => x.AddPlayerAsync(bookingId, request))
            .ReturnsAsync(Result.Failure<bool>(error));

        var result = await _controller.AddPlayer(bookingId, request);

        var conflict = Assert.IsType<ConflictObjectResult>(result);
        var problem = Assert.IsType<ProblemDetails>(conflict.Value);
        Assert.Equal(409, problem.Status);
        Assert.Equal("Booking.InvalidPlayerRank", problem.Extensions["errorCode"]);
    }

    [Fact]
    public async Task GetBooking_ReturnsOk()
    {
        var bookingId = Guid.NewGuid();
        var booking = new BookingResponse(bookingId, Guid.NewGuid(), Guid.NewGuid(),
            "Confirmed", "Matchmaking", "Doubles", DateTime.UtcNow, DateTime.UtcNow.AddHours(1),
            50, "EUR", new List<PlayerResponse>());

        _mockService.Setup(x => x.GetBookingResponseAsync(bookingId))
            .ReturnsAsync(Result.Success(booking));

        var result = await _controller.GetBooking(bookingId);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetBooking_WhenNotFound_Returns404()
    {
        var bookingId = Guid.NewGuid();

        _mockService.Setup(x => x.GetBookingResponseAsync(bookingId))
            .ReturnsAsync(new Result<BookingResponse>(null, false, ApplicationErrors.BookingNotFound(Guid.Empty)));

        var result = await _controller.GetBooking(bookingId);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UnknownError_Returns500()
    {
        var bookingId = Guid.NewGuid();
        var request = new AddPlayerRequest(Guid.NewGuid(), "Intermediate");
        var error = new Error("Unknown", "Something went wrong");

        _mockService.Setup(x => x.AddPlayerAsync(bookingId, request))
            .ReturnsAsync(Result.Failure<bool>(error));

        var result = await _controller.AddPlayer(bookingId, request);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(serverError.Value);
        Assert.Equal("Unknown", problem.Extensions["errorCode"]);
    }
}
