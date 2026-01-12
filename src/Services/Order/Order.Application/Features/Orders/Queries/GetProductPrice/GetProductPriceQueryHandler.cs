using MediatR;
using Order.Application.Interfaces;
using Order.Domain.Entities;
using Order.Domain.ValueObjects;

namespace Order.Application.Features.Orders.Queries.GetProductPrice;

public record GetProductPriceQuery(Guid ProductId) : IRequest<decimal?>;

public class GetProductPriceQueryHandler : IRequestHandler<GetProductPriceQuery, decimal?>
{
    private readonly ICatalogServiceClient _catalogClient;

    public GetProductPriceQueryHandler(ICatalogServiceClient catalogClient)
    {
        _catalogClient = catalogClient;
    }

    public async Task<decimal?> Handle(GetProductPriceQuery request, CancellationToken cancellationToken)
    {
        return await _catalogClient.GetProductPriceAsync(request.ProductId, cancellationToken);
    }
}
