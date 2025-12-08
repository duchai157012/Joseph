using Auth.Application.Features.Commands.Login;
using Auth.Application.Features.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginCommand command)
    {
        var token = await _mediator.Send(command);
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { UserId = result });
    }
}
