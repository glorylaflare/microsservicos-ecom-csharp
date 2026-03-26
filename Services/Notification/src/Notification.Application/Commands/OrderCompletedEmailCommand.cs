using BuildingBlocks.Contracts;
using MediatR;

namespace Notification.Application.Commands;

public record OrderCompletedEmailCommand(string Email, int OrderId, List<OrderItemDto> Items) : IRequest<Unit>;
