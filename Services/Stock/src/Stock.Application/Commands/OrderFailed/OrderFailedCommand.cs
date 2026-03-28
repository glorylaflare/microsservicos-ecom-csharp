using BuildingBlocks.Contracts;
using MediatR;

namespace Stock.Application.Commands.OrderFailed;

public record OrderFailedCommand(List<OrderItemDto> Items) : IRequest<Unit>;