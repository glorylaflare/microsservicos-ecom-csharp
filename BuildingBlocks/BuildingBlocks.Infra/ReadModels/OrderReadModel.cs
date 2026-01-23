namespace BuildingBlocks.Infra.ReadModels;

public class OrderReadModel
{
    public int Id { get; init; }
    public List<OrderItemReadModel> Items { get; set; } = new();
    public StatusReadModel Status { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
public enum StatusReadModel
{
    Pending,
    Reserved,
    Completed,
    Cancelled
}