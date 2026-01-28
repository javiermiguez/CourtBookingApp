namespace Bookings.Domain;

// Value Object - Price
public record Price(decimal Amount, Currency Currency)
{
    public Price Add(Price other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException("Cannot add prices with different currencies.");
        }

        return new Price(Amount + other.Amount, Currency);
    }
}
