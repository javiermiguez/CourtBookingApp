using Bookings.Application.DTOs.Requests;
using Bookings.Application.DTOs.Responses;
using Bookings.Common;

namespace Bookings.Application;

public interface IBookingAppService
{
    Task<Result<Guid>> CreateBookingAsync(Guid userId, CreateBookingRequest request);
    Task<Result> AddPlayerAsync(Guid bookingId, AddPlayerRequest request);
    Task<Result<BookingResponse>> GetBookingResponseAsync(Guid id);
    Task<Result> DeleteBookingAsync(Guid id, Guid userId);
}
