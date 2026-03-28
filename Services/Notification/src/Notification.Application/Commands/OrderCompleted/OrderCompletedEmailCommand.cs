using BuildingBlocks.Contracts;
using MediatR;

namespace Notification.Application.Commands.OrderCompleted;

public record OrderCompletedEmailCommand(string Email, int OrderId, List<OrderItemDto> Items) : IRequest<Unit>;
