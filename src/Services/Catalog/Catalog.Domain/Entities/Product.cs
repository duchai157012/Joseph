using System;

namespace Catalog.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PictureUrl { get; set; } = string.Empty;
    public int AvailableStock { get; set; }

    // Audit info
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
