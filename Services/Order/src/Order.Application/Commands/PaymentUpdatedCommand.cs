using MediatR;

namespace Order.Application.Commands;

public record PaymentUpdatedCommand(int OrderId, string Status, string CheckoutUrl) : IRequest<Unit>;
