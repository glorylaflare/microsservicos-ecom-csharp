using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record OrderFailedData
{
    public List<OrderItemDto> Items { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Email { get; set; }

    [JsonConstructor]
    public OrderFailedData(List<OrderItemDto> items, string reason, string email)
    {
        Items = items;
        Reason = reason;
        Email = email;
    }
}
