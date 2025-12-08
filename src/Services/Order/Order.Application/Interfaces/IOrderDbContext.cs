using Order.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Order.Application.Interfaces;

public interface IOrderDbContext
{
    DbSet<Domain.Entities.Order> Orders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
