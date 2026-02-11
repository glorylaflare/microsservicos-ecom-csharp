using System.Text.Json.Serialization;
namespace BuildingBlocks.Contracts.Datas;

public record PaymentUpdatedData
{
    public int PaymentId { get; init; }
    public int OrderId { get; init; }
    public string CheckoutUrl { get; init; }
    public string Status { get; init; }

    [JsonConstructor]
    public PaymentUpdatedData(int paymentId, int orderId, string checkoutUrl, string status)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        CheckoutUrl = checkoutUrl;
        Status = status;
    }
}