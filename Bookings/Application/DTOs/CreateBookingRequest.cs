using System.ComponentModel;

namespace Bookings.Application.DTOs.Requests;

public record CreateBookingRequest(

    [property: DefaultValue("123e4567-e89b-12d3-a456-426614174000")]
    Guid CourtId,

    DateTime StartTime,
    DateTime EndTime,

    [property: DefaultValue("Direct")]
    string Modality,

    [property: DefaultValue("Singles")]
    string GameType,

    [property: DefaultValue("Intermediate")]
    string PlayerRank,

    [property: DefaultValue("30.0")]
    decimal CourtPricePerHour);
