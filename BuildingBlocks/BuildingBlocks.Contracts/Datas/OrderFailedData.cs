using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record OrderFailedData
{
    public List<OrderItemDto> Items { get; init; }
    public string? Reason { get; init; } = string.Empty;

    [JsonConstructor]
    public OrderFailedData(int orderId, List<OrderItemDto> items, string? reason)
    {
        Items = items;
        Reason = reason;
    }
}
