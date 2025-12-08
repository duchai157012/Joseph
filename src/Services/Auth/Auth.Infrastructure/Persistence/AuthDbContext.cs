using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistence;

public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }
}
