using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<List<Product>>
{
}
