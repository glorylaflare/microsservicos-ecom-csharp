using MediatR;
namespace Order.Application.Commands;
public record StockReservedCommand(int OrderId, decimal TotalAmount) : IRequest<Unit>;