using MediatR;
namespace Order.Application.Commands;
public record StockRejectedCommand(int OrderId, string? Reason) : IRequest<Unit>;