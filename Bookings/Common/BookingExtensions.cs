using Bookings.Application.DTOs.Responses;
using Bookings.Domain;

namespace Bookings.Common;

public static class BookingExtensions
{
    public static BookingResponse ToResponse(this Booking booking)
    {
        ArgumentNullException.ThrowIfNull(booking);

        return new BookingResponse(
            booking.Id,
            booking.UserId,
            booking.CourtId,
            booking.Status.ToString(),
            booking.Configuration.Modality.ToString(),
            booking.Configuration.GameType.ToString(),
            booking.BookingPeriod.Start,
            booking.BookingPeriod.End,
            booking.Price.Amount,
            booking.Price.Currency.ToString(),
            booking.Players.Select(p => p.ToResponse()));
    }

    public static PlayerResponse ToResponse(this Booking.Player player)
    {
        ArgumentNullException.ThrowIfNull(player);

        return new PlayerResponse(
            player.UserId,
            player.Rank.ToString(),
            player.IsRequester);
    }
}
