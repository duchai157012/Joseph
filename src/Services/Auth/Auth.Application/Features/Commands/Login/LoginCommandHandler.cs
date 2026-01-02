using Auth.Application.Common.Interfaces;
using Auth.Domain.Entities;
using BuildingBlocks.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Features.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            throw new UnauthorizedException("Invalid credentials");

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
            throw new UnauthorizedException("Invalid credentials");

        return _tokenService.GenerateToken(user);
    }
}
