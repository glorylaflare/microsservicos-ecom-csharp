using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record OrderUpdatedData
{
    public int Id { get; init; }
    public string Status { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime? UpdatedAt { get; init; }

    [JsonConstructor]
    public OrderUpdatedData(int id, string status, decimal totalAmount, DateTime? updatedAt)
    {
        Id = id;
        Status = status;
        TotalAmount = totalAmount;
        UpdatedAt = updatedAt;
    }
}
