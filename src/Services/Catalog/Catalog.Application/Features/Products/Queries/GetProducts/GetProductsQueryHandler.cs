using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<Product>>
{
    private readonly ICatalogDbContext _context;

    public GetProductsQueryHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
    }
}
