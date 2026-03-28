using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Commands.ProcessPayment;
using Payment.Application.Queries.GetAllPayments;
using Payment.Application.Queries.GetPaymentById;
using System.ComponentModel.DataAnnotations;
namespace Payment.Api.Controllers;

[Authorize]
[Route("api/payments")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllPaymentsQuery());
        return result.IsFailed
            ? NotFound(result.Errors)
            : Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([Range(1, int.MaxValue)] int id)
    {
        var result = await _mediator.Send(new GetPaymentByIdQuery(id));
        return result.IsFailed
            ? NotFound(result.Errors)
            : Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> WebhookAsync([FromBody] ProcessPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsFailed
            ? BadRequest(result.Errors)
            : Ok(result.Value);
    }
}