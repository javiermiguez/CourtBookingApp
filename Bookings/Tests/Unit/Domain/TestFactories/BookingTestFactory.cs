using Bookings.Domain;
using GameType = Bookings.Domain.GameType;

namespace Bookings.Tests.Unit.Domain.TestFactories;

public class BookingTestFactory
{
    public static Booking CreateTestBooking(
        Guid? userId = null,
        Guid? courtId = null,
        BookingConfiguration? configuration = null,
        Period? period = null,
        PlayerRank? rank = null,
        decimal pricePerHour = 20.0m,
        Currency? currency = null)
    {
        if (userId == null)
        {
            userId = Guid.NewGuid();
        }

        if (courtId == null)
        {
            courtId = Guid.NewGuid();
        }

        if (configuration == null)
        {
            configuration = new BookingConfiguration(
                BookingModality.Direct, GameType.Singles);
        }

        if (period == null)
        {
            period = new Period(
                DateTime.UtcNow.AddHours(1),
                DateTime.UtcNow.AddHours(3));
        }

        if (rank == null)
        {
            rank = PlayerRank.Beginner;
        }

        if (currency == null)
        {
            currency = Currency.EUR;
        }

        return Booking.Create(
            userId.Value,
            courtId.Value,
            configuration,
            period,
            rank.Value,
            pricePerHour,
            currency.Value);
    }
}
