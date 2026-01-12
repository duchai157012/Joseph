namespace Order.Application.Interfaces;

public interface ICatalogServiceClient
{
    Task<ProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<decimal?> GetProductPriceAsync(Guid productId, CancellationToken cancellationToken = default);
}

public record ProductDto(Guid Id, string Name, decimal Price, int StockQuantity);
