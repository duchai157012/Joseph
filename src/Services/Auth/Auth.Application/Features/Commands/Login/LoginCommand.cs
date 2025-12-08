using MediatR;

namespace Auth.Application.Features.Commands.Login;

public class LoginCommand : IRequest<string>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
