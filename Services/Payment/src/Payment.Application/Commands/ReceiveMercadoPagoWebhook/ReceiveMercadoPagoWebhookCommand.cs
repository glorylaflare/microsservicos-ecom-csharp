using FluentResults;
using MediatR;

namespace Payment.Application.Commands.ReceiveMercadoPagoWebhook;

public record ReceiveMercadoPagoWebhookCommand(string Payload) : IRequest<Result<Unit>>;