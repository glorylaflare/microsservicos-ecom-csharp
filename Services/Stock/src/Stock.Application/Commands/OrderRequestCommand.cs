using BuildingBlocks.Contracts;
using MediatR;

namespace Stock.Application.Commands;

public record OrderRequestCommand(int OrderId, List<OrderItemDto> Items) : IRequest<Unit>;
