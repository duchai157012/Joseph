using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Orders.Commands.CreateOrder;

namespace Order.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateOrderCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { Id = id });
    }
}
