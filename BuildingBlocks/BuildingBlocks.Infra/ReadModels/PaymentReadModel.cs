namespace BuildingBlocks.Infra.ReadModels;

public class PaymentReadModel
{
    public int Id { get; init; }
    public int OrderId { get; init; }
    public decimal Amount { get; init; }
    public string? Status { get; init; }
    public string? CheckoutUrl { get; init; }
    public string? MercadoPagoPreference { get; init; }
    public DateTime ExpirationDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}