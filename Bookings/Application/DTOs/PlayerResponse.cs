namespace Bookings.Application.DTOs.Responses;

public record PlayerResponse(
    Guid UserId,
    string Rank,
    bool IsRequester);
