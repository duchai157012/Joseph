using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Orders.Queries.GetProductPrice;
using MediatR;

namespace Order.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets product price from Catalog service using resilient HTTP client
    /// Demonstrates Polly retry and circuit breaker patterns in action
    /// </summary>
    [HttpGet("{productId}/price")]
    public async Task<IActionResult> GetProductPrice(Guid productId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching price for product {ProductId} via Catalog service", productId);

        var price = await _mediator.Send(new GetProductPriceQuery(productId), cancellationToken);

        if (price == null)
        {
            return NotFound(new { message = $"Product {productId} not found in Catalog" });
        }

        return Ok(new { productId, price });
    }
}
