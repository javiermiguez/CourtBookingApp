namespace Bookings.Application.DTOs.Responses;

public record BookingResponse(
    Guid Id,
    Guid UserId,
    Guid CourtId,
    string Status,
    string Modality,
    string GameType,
    DateTime StartTime,
    DateTime EndTime,
    decimal PriceAmount,
    string PriceCurrency,
    IEnumerable<PlayerResponse> Players);
