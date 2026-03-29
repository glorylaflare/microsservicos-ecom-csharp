using FluentResults;
using MediatR;
using Payment.Domain.Models;

namespace Payment.Application.Commands.ProcessPayment;

public record ProcessPaymentCommand(WebhookPayload WebhookPayload) : IRequest<Result<Unit>>;