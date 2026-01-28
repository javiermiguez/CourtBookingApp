namespace Bookings.Domain;

// Value Object - Period
public record Period
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public TimeSpan Duration => End - Start;

    public Period(DateTime start, DateTime end)
    {
        if (end <= start)
        {
            throw new ArgumentException("End date must be after start date");
        }

        Start = start;
        End = end;
    }

    public bool OverlapsWith(Period other)
    {
        return Start < other.End && End > other.Start;
    }
}
