using FluentResults;
using MediatR;

namespace Payment.Application.Commands.Handlers;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<Unit>>
{
    public Task<Result<Unit>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Processing payment of type: " + request.Type);
        return Task.FromResult(Result.Ok(Unit.Value));
    }
}
