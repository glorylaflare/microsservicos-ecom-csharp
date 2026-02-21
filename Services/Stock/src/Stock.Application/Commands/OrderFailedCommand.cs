using BuildingBlocks.Contracts;
using MediatR;

namespace Stock.Application.Commands;

public record OrderFailedCommand(List<OrderItemDto> Items) : IRequest<Unit>;