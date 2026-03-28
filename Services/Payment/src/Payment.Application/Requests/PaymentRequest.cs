using BuildingBlocks.Contracts;

namespace Payment.Application.Requests;

public record PaymentRequest(
    Guid EventId,
    int OrderId,
    decimal TotalAmount,
    List<ProductItemDto> Items
);

