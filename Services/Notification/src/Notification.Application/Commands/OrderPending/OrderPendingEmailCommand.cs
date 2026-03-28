using BuildingBlocks.Contracts;
using MediatR;

namespace Notification.Application.Commands.OrderPending;

public record OrderPendingEmailCommand(string Email, int OrderId, List<OrderItemDto> Items, string CheckoutUrl) : IRequest<Unit>;
