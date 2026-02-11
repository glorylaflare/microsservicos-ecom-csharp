using BuildingBlocks.Infra.MongoReadModels;

namespace Order.Application.Responses;

public record GetOrderComposeResponse(
    GetUserResponse User, 
    GetOrderResponse Order,
    GetPaymentResponse Payment
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

public record GetPaymentResponse(
    int PaymentId,
    string Status
);