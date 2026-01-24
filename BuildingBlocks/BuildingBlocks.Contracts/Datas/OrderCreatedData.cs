using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record OrderCreatedData
{
    public int Id { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }

    [JsonConstructor]
    public OrderCreatedData(int id, List<OrderItemDto> items, string status, DateTime createdAt)
    {
        Id = id;
        Items = items;
        Status = status;
        CreatedAt = createdAt;
    }
}
