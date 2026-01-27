namespace Bookings.Domain
{
    public struct BookingConfiguration
    {
        public BookingMode Mode { get; }
        public MatchType MatchType { get; }

        public BookingConfiguration(BookingMode mode, MatchType matchType)
        {
            Mode = mode;
            MatchType = matchType;
        }
    }
}
