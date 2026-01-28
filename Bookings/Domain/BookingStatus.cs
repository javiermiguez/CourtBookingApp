namespace Bookings.Domain;

public enum BookingStatus
{
    Pending = 0,
    WaitingForPlayers = 1,
    PendingPayment = 2,
    Confirmed = 3,
    Cancelled = 4
}
