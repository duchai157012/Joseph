using Catalog.Application.Features.Products.Commands.CreateProduct;
using Catalog.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetProductsQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = id }, id);
    }
}
