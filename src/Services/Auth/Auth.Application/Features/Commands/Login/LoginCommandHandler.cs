using Auth.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Application.Features.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null) throw new Exception("User not found");

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result) throw new Exception("Invalid password");

        // Generate Token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("ThisIsASecretKeyForJwtTokenGenerationYOUR_SECRET_KEY_HERE");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                 new Claim(ClaimTypes.Name, user.UserName),
                 new Claim(ClaimTypes.NameIdentifier, user.Id)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
