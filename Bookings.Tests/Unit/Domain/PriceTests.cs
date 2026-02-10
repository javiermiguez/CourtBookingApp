using Bookings.Domain;

namespace Bookings.Tests.Unit.Domain;

public class PriceTests
{
    [Fact]
    public void Create_ValidPrice_ShouldInitializeCorrectly()
    {
        var price = new Price(100.50m, Currency.EUR);

        Assert.Equal(100.50m, price.Amount);
        Assert.Equal(Currency.EUR, price.Currency);
    }

    [Fact]
    public void Create_PriceWithNegativeAmount_ShouldThrowException()
    {
        decimal negativeAmount = -10.0m;

        var exception = Assert.Throws<DomainException>(() =>
            new Price(negativeAmount, Currency.EUR));
    }
}
