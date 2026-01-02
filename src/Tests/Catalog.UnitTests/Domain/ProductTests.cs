using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;
using FluentAssertions;

namespace Catalog.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        var name = "Test Product";
        var description = "Test Description";
        var price = 99.99m;
        var pictureUrl = "http://test.com/image.jpg";
        var stock = 10;
        var createdAt = new DateTime(2025, 1, 1);

        var product = Product.Create(name, description, price, pictureUrl, stock, createdAt);

        product.Should().NotBeNull();
        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Amount.Should().Be(price);
        product.Price.Currency.Should().Be("USD");
        product.AvailableStock.Should().Be(stock);
        product.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldThrowException()
    {
        var action = () => Product.Create("Test", "Desc", 10.0m, "url", -5, DateTime.UtcNow);

        action.Should().Throw<ArgumentException>()
            .WithMessage("*stock cannot be negative*");
    }

    [Fact]
    public void UpdatePrice_ShouldUpdatePriceAndModifiedDate()
    {
        var product = Product.Create("Test", "Desc", 10.0m, "url", 10, DateTime.UtcNow);
        var newPrice = 20.0m;
        var modifiedAt = DateTime.UtcNow.AddMinutes(5);

        product.UpdatePrice(newPrice, modifiedAt);

        product.Price.Amount.Should().Be(newPrice);
        product.LastModifiedAt.Should().Be(modifiedAt);
    }

    [Fact]
    public void ReduceStock_WithSufficientStock_ShouldReduceSuccessfully()
    {
        var product = Product.Create("Test", "Desc", 10.0m, "url", 10, DateTime.UtcNow);

        product.ReduceStock(3, DateTime.UtcNow);

        product.AvailableStock.Should().Be(7);
    }

    [Fact]
    public void ReduceStock_WithInsufficientStock_ShouldThrowException()
    {
        var product = Product.Create("Test", "Desc", 10.0m, "url", 5, DateTime.UtcNow);

        var action = () => product.ReduceStock(10, DateTime.UtcNow);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");
    }
}
