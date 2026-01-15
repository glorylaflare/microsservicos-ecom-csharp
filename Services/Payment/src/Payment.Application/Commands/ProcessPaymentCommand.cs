using FluentResults;
using MediatR;

namespace Payment.Application.Commands;

public record ProcessPaymentCommand() : IRequest<Result<Unit>>;