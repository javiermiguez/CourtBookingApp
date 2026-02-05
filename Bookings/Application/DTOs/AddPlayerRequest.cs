namespace Bookings.Application.DTOs.Requests;

public record AddPlayerRequest(
    Guid PlayerId,
    string PlayerRank);
