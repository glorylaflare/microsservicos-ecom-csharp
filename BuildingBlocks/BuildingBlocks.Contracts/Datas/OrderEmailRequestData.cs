using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record OrderEmailRequestData
{
    public string Email { get; init; }
    public int OrderId { get; init; }
    public List<OrderItemDto> Items { get; init; }
    public string? CheckoutUrl { get; init; }
    public string? Reason { get; init; } = string.Empty;
    public OrderPaymentStatus Status { get; init; }

    [JsonConstructor]
    public OrderEmailRequestData(OrderPaymentStatus status, string email, int orderId, List<OrderItemDto> items, string? checkoutUrl, string? reason)
    {
        Status = status;
        Email = email;
        OrderId = orderId;
        Items = items;
        CheckoutUrl = checkoutUrl;
        Reason = reason;
    }

    public static OrderEmailRequestData Failed(string email, int orderId, List<OrderItemDto> items, string reason) =>
        new(OrderPaymentStatus.Failed, email, orderId, items, null, reason);

    public static OrderEmailRequestData Pending(string email, int orderId, List<OrderItemDto> items, string checkoutUrl) =>
        new(OrderPaymentStatus.Pending, email, orderId, items, checkoutUrl, null);

    public static OrderEmailRequestData Completed(string email, int orderId, List<OrderItemDto> items) =>
        new(OrderPaymentStatus.Completed, email, orderId, items, null, null);
}

public enum OrderPaymentStatus
{
    Failed,
    Pending,
    Completed
}