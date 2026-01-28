using Bookings.Common;

namespace Bookings.Domain;

public static class BookingErrors
{
    public static readonly Error OnlyMatchmakingCanAddPlayers = new(
        "Booking.OnlyMatchmakingCanAddPlayers",
        "Only matchmaking bookings can add players");

    public static readonly Error BookingNotWaiting = new(
        "Booking.BookingNotWaiting",
        "Booking is not waiting for players");

    public static readonly Error InvalidPlayerRank = new(
        "Booking.InvalidPlayerRank",
        "Player rank does not match booking requirements");

    public static readonly Error PlayerAlreadyInBooking = new(
        "Booking.PlayerAlreadyInBooking",
        "Player is already in this booking");
}
