using MediatR;

namespace Catalog.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PictureUrl { get; set; } = string.Empty;
    public int AvailableStock { get; set; }
}
