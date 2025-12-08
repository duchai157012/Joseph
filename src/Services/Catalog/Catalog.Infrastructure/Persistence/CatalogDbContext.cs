using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public class CatalogDbContext : DbContext, ICatalogDbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}
