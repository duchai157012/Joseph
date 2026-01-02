using Catalog.Domain.ValueObjects;
using FluentAssertions;

namespace Catalog.UnitTests.Domain;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldCreateMoney()
    {
        var money = Money.Create(100.50m, "USD");

        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowException()
    {
        var action = () => Money.Create(-10.0m, "USD");

        action.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldAddAmounts()
    {
        var money1 = Money.Create(100m, "USD");
        var money2 = Money.Create(50m, "USD");

        var result = money1.Add(money2);

        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("USD");
    }

    [Fact]  
    public void Add_WithDifferentCurrency_ShouldThrowException()
    {
        var money1 = Money.Create(100m, "USD");
        var money2 = Money.Create(50m, "EUR");

        var action = () => money1.Add(money2);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void Subtract_ValidOperation_ShouldSubtractAmounts()
    {
        var money1 = Money.Create(100m, "USD");
        var money2 = Money.Create(30m, "USD");

        var result = money1.Subtract(money2);

        result.Amount.Should().Be(70m);
    }

    [Fact]
    public void Subtract_ResultingInNegative_ShouldThrowException()
    {
        var money1 = Money.Create(50m, "USD");
        var money2 = Money.Create(100m, "USD");

        var action = () => money1.Subtract(money2);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*negative amount*");
    }
}
