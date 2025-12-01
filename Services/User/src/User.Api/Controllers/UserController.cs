using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using User.Application.Commands;
using User.Application.Queries;

namespace User.Api.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return result.IsFailed
            ? NotFound(result.Errors.Select(e => e.Message))
            : Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserById([Range(1, int.MaxValue)]int id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        return result.IsFailed
            ? NotFound(result.Errors.Select(e => e.Message))
            : Ok(result.Value);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsFailed
            ? BadRequest(result.Errors.Select(e => e.Message))
            : CreatedAtAction("GetUserById", "User", new { id = result.Value }, result);
    }
}
