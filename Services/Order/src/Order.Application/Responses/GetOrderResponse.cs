using BuildingBlocks.Infra.ReadModels;

namespace Order.Application.Responses;

public record GetOrderResponse(
    int Id,
    IReadOnlyList<OrderItemReadModel> Items,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
