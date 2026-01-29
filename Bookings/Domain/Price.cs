namespace Bookings.Domain;

// Value Object - Price
public record Price
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Price(decimal amount, Currency currency)
    {
        if (amount < 0)
        {
            throw new DomainException("Price amount cannot be negative");
        }

        Amount = amount;
        Currency = currency;
    }

    public Price Add(Price other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException("Cannot add prices with different currencies");
        }

        return new Price(Amount + other.Amount, Currency);
    }
}
