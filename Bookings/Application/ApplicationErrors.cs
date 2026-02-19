using Bookings.Common;

namespace Bookings.Application;

public static class ApplicationErrors
{
    public static Error InvalidModality(string value) =>
        new("InvalidModality", $"Invalid modality: {value}");

    public static Error InvalidGameType(string value) =>
        new("InvalidGameType", $"Invalid game type: {value}");

    public static Error InvalidPlayerRank(string value) =>
        new("InvalidPlayerRank", $"Invalid player rank: {value}");

    public static Error InvalidBookingData(string details) =>
        new("InvalidBookingData", details);

    public static Error BookingNotFound(Guid id) =>
        new("BookingNotFound", $"Booking {id} not found");

    public static Error DatabaseError(string? details = null) =>
        new("DatabaseError", details ?? "Database operation failed");

    public static Error DomainError(string message) =>
        new("DomainError", message);

    public static Error Unauthorized(string details) =>
        new("Unauthorized", details);
}
