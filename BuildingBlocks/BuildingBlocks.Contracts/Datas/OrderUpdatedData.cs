using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record OrderUpdatedData
{
    public int OrderId { get; init; }
    public string Status { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime? UpdatedAt { get; init; }

    [JsonConstructor]
    public OrderUpdatedData(int orderId, string status, decimal totalAmount, DateTime? updatedAt)
    {
        OrderId = orderId;
        Status = status;
        TotalAmount = totalAmount;
        UpdatedAt = updatedAt;
    }
}
