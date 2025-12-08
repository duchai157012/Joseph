using Order.Application.Interfaces;
using Order.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Order.Infrastructure.Persistence;

public class OrderDbContext : DbContext, IOrderDbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Order> Orders { get; set; }
}
