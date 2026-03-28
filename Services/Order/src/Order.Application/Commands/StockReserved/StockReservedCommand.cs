using MediatR;

namespace Order.Application.Commands.StockReserved;

public record StockReservedCommand(int OrderId, decimal TotalAmount) : IRequest<Unit>;