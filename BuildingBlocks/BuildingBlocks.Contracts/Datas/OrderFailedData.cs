using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record OrderFailedData
{
    public int OrderId { get; init; }
    public List<OrderItemDto> Items { get; init; }
    public string? Reason { get; init; } = string.Empty;

    [JsonConstructor]
    public OrderFailedData(int orderId, List<OrderItemDto> items, string? reason)
    {
        OrderId = orderId;
        Items = items;
        Reason = reason;
    }
}
