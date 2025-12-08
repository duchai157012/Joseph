using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Interfaces;

public interface ICatalogDbContext
{
    DbSet<Product> Products { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
