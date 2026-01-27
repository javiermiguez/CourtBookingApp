namespace Bookings.Domain
{
    public struct Price
    {
        public decimal Amount { get; }
        public Currency Currency { get; }

        public Price(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public Price Add(Price other)
        {
            if (Currency != other.Currency)
            {
                throw new InvalidOperationException("Cannot add prices with different currencies.");
            }

            return new Price(Amount + other.Amount, Currency);
        }
    }
}
