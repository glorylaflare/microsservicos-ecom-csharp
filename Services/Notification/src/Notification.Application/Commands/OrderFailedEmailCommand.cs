using BuildingBlocks.Contracts;
using MediatR;

namespace Notification.Application.Commands;

public record OrderFailedEmailCommand(string Email, int OrderId, List<OrderItemDto> Items, string Reason) : IRequest<Unit>;
