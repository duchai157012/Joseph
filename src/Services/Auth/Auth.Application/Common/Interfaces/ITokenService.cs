using Auth.Domain.Entities;

namespace Auth.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user);
}
