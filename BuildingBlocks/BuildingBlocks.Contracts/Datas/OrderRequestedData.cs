using System.Text.Json.Serialization;
namespace BuildingBlocks.Contracts.Datas;

public record OrderRequestedData
{
    public int OrderId { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
    [JsonConstructor]
    public OrderRequestedData(int orderId, List<OrderItemDto> items)
    {
        OrderId = orderId;
        Items = items;
    }
}