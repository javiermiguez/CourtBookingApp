namespace Bookings.Application.DTOs.Requests;

public record CreateBookingRequest(
    Guid CourtId,
    DateTime StartTime,
    DateTime EndTime,
    string Modality,
    string GameType,
    string PlayerRank,
    decimal CourtPricePerHour);
