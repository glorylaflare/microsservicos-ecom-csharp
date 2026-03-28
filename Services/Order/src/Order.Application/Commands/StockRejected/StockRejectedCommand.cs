using MediatR;

namespace Order.Application.Commands.StockRejected;

public record StockRejectedCommand(int OrderId, string? Reason) : IRequest<Unit>;