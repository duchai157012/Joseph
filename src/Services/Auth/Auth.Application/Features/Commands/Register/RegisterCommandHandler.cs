using Auth.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Features.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return user.Id;
    }
}
