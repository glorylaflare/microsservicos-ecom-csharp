using BuildingBlocks.Infra.MongoReadModels;

namespace Order.Application.Responses;

public record GetOrderResponse(
    int Id,
    IReadOnlyList<OrderItemReadModel> Items,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);