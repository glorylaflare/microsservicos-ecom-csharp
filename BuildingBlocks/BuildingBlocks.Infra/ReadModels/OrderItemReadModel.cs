namespace BuildingBlocks.Infra.ReadModels;

public class OrderItemReadModel
{
    public int OrderId { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}