namespace BuildingBlocks.Infra.ReadModels;

public class PaymentReadModel
{
    public int Id { get; init; }
    public int OrderId { get; init; }
    public decimal Amount { get; init; }
    public PaymentStatusReadModel Status { get; init; }
    public string? CheckoutUrl { get; init; }
    public DateTime? CreatedDate { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public enum PaymentStatusReadModel
{
    Pending,
    Paid,
    Failed
}
