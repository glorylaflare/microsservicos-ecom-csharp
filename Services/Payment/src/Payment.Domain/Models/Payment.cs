using BuildingBlocks.SharedKernel.Common;
namespace Payment.Domain.Models;

public class Payment : EntityBase
{
    public int OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? CheckoutUrl { get; private set; }
    public string? MercadoPagoPreference { get; private set; }
    public DateTime? ExpirationDate { get; private set; }

    public Payment() { }

    public Payment(int orderId, decimal amount, string? checkoutUrl, string mercadoPagoPreference, DateTime? expirationDate)
    {
        OrderId = orderId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CheckoutUrl = checkoutUrl;
        MercadoPagoPreference = mercadoPagoPreference;
        ExpirationDate = expirationDate;
    }

    public void SetStatus(PaymentStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed
}