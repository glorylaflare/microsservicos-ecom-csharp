using BuildingBlocks.Infra.MongoReadModels;

namespace Order.Application.Responses;

public record GetOrderComposeRespose(
    GetUserResponse User, 
    GetOrderResponse Order
);

public record GetOrderResponse(
    int Id,
    IReadOnlyList<OrderItemReadModel> Items,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record GetUserResponse(
    string Username, 
    string Email
);