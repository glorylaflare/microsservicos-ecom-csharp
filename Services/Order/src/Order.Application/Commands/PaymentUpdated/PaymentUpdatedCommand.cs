using MediatR;

namespace Order.Application.Commands.PaymentUpdated;

public record PaymentUpdatedCommand(int OrderId, string Status, string CheckoutUrl) : IRequest<Unit>;
