using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock.Application.Commands;
using Stock.Application.Queries;
using System.ComponentModel.DataAnnotations;
namespace Stock.Api.Controllers;

[Authorize]
[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetAllProducts()
    {
        var result = await _mediator.Send(new GetAllProductsQuery());
        return result.IsFailed
            ? NotFound(result.Errors.Select(e => e.Message))
            : Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetProductById([Range(1, int.MaxValue)] int id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        return result.IsFailed
            ? NotFound(result.Errors.Select(e => e.Message))
            : Ok(result.Value);
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsFailed
            ? BadRequest(result.Errors.Select(e => e.Message))
            : CreatedAtAction(nameof(GetProductById), new { id = result.Value }, result);
    }

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsFailed
            ? BadRequest(result.Errors.Select(e => e.Message))
            : NoContent();
    }

    [HttpPut("restock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RestockProduct([FromBody] UpdateStockCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsFailed
            ? BadRequest(result.Errors.Select(e => e.Message))
            : NoContent();
    }
}