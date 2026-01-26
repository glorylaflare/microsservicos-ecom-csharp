using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Commands;
using Order.Application.Queries;
using System.ComponentModel.DataAnnotations;
namespace Order.Api.Controllers;

[Authorize]
[Route("api/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetOrderById([Range(1, int.MaxValue)] int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        return result.IsFailed
            ? NotFound(result.Errors)
            : Ok(result.Value);
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsFailed
            ? BadRequest(result.Errors)
            : CreatedAtAction(nameof(GetOrderById), new { id = result.Value }, result);
    }
}