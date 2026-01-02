using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;
    public string PictureUrl { get; private set; } = string.Empty;
    public int AvailableStock { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }

    private Product() { }

    public static Product Create(
        string name,
        string description,
        decimal price,
        string pictureUrl,
        int availableStock,
        DateTime createdAt,
        string currency = "USD")
    {
        ArgumentNullException.ThrowIfNull(name);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (availableStock < 0)
            throw new ArgumentException("Available stock cannot be negative", nameof(availableStock));

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Price = Money.Create(price, currency),
            PictureUrl = pictureUrl?.Trim() ?? string.Empty,
            AvailableStock = availableStock,
            CreatedAt = createdAt,
            LastModifiedAt = null
        };

        return product;
    }

    public void UpdatePrice(decimal newPrice, DateTime modifiedAt)
    {
        Price = Money.Create(newPrice, Price.Currency);
        LastModifiedAt = modifiedAt;
    }

    public void UpdateStock(int newStock, DateTime modifiedAt)
    {
        if (newStock < 0)
            throw new ArgumentException("Stock cannot be negative", nameof(newStock));

        AvailableStock = newStock;
        LastModifiedAt = modifiedAt;
    }

    public void ReduceStock(int quantity, DateTime modifiedAt)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (AvailableStock < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {AvailableStock}, Requested: {quantity}");

        AvailableStock -= quantity;
        LastModifiedAt = modifiedAt;
    }

    public void AddStock(int quantity, DateTime modifiedAt)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        AvailableStock += quantity;
        LastModifiedAt = modifiedAt;
    }

    public void UpdateDetails(string name, string description, DateTime modifiedAt)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        LastModifiedAt = modifiedAt;
    }
}
