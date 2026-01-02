using Catalog.Application.Common.Interfaces;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly ICatalogDbContext _context;
    private readonly IDateTimeProvider _dateTime;

    public CreateProductCommandHandler(ICatalogDbContext context, IDateTimeProvider dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            name: request.Name,
            description: request.Description,
            price: request.Price,
            pictureUrl: request.PictureUrl,
            availableStock: request.AvailableStock,
            createdAt: _dateTime.UtcNow);

        _context.Products.Add(product);

        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
