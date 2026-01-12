using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Order.Application.Interfaces;

namespace Order.Infrastructure.Clients;

public class CatalogServiceClient : ICatalogServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogServiceClient> _logger;

    public CatalogServiceClient(HttpClient httpClient, ILogger<CatalogServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching product {ProductId} from Catalog.API", productId);

            var response = await _httpClient.GetAsync($"/api/products/{productId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch product {ProductId}. Status: {StatusCode}",
                    productId, response.StatusCode);
                return null;
            }

            var product = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);

            _logger.LogInformation("Successfully fetched product {ProductId}: {ProductName}",
                productId, product?.Name);

            return product;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while fetching product {ProductId}", productId);
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request timeout while fetching product {ProductId}", productId);
            throw;
        }
    }

    public async Task<decimal?> GetProductPriceAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await GetProductByIdAsync(productId, cancellationToken);
        return product?.Price;
    }
}
