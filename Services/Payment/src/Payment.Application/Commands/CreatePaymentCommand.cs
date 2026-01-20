using BuildingBlocks.Contracts;
using MediatR;

namespace Payment.Application.Commands;

public record CreatePaymentCommand(Guid EventId, int OrderId, decimal TotalAmount, List<ProductItemDto> Items) : IRequest<Unit>;
