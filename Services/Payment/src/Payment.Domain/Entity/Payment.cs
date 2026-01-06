namespace Payment.Domain.Entity;

public class Payment
{

    public Guid Id { get; private set; }
    public int OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? CheckoutUrl { get; private set; }
    public DateTime? CreatedDate { get; private set; }

    public Payment() { }

    public Payment(int orderId, decimal amount, PaymentStatus status, string? checkoutUrl)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        Status = status;
        CheckoutUrl = checkoutUrl;
        CreatedDate = DateTime.UtcNow;
    }
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed
}