using BuildingBlocks.SharedKernel.Common;

namespace Payment.Domain.Models;

public class Payment : EntityBase
{
    public int OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? CheckoutUrl { get; private set; }

    public Payment() { }

    public Payment(int orderId, decimal amount, string? checkoutUrl)
    {
        OrderId = orderId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CheckoutUrl = checkoutUrl;
    }

    public void MarkAsPaid()
    {
        Status = PaymentStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed
}