using BuildingBlocks.Contracts;
using FluentResults;
using MediatR;

namespace Payment.Application.Commands.CreatePayment;

public record CreatePaymentCommand(
    Guid EventId, 
    int OrderId, 
    decimal TotalAmount, 
    List<ProductItemDto> Items
) : IRequest<Result<Unit>>;