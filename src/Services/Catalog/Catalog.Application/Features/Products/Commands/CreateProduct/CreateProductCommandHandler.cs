using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly ICatalogDbContext _context;

    public CreateProductCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            PictureUrl = request.PictureUrl,
            AvailableStock = request.AvailableStock,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
