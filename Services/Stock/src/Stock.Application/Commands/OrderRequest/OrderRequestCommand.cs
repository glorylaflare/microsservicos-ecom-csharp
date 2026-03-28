using BuildingBlocks.Contracts;
using MediatR;

namespace Stock.Application.Commands.OrderRequest;

public record OrderRequestCommand(int OrderId, List<OrderItemDto> Items) : IRequest<Unit>;