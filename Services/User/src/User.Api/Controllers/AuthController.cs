using MediatR;
using Microsoft.AspNetCore.Mvc;
using User.Application.Commands;

namespace User.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
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

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Login([FromBody] AuthenticateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsFailed
            ? BadRequest(result.Errors.Select(e => e.Message))
            : Ok(result.Value);
    }
}
