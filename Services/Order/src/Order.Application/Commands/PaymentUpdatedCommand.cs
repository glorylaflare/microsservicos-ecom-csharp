using MediatR;

namespace Order.Application.Commands;

public record PaymentUpdatedCommand(int OrderId, string Status) : IRequest<Unit>;
