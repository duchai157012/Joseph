using MediatR;

namespace Auth.Application.Features.Commands.Register;

public class RegisterCommand : IRequest<string>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
