using Order.Domain.Models;

namespace Order.Application.Responses;

public record GetOrderResponse(
    int Id,
    List<OrderItem> Items,
    decimal TotalAmount,
    Status Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
