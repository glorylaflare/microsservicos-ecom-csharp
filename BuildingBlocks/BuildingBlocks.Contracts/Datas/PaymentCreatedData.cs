using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record PaymentCreatedData
{
    public int PaymentId { get; init; }
    public string CheckoutUrl { get; init; }

    [JsonConstructor]
    public PaymentCreatedData(int paymentId, string checkoutUrl)
    {
        PaymentId = paymentId;
        CheckoutUrl = checkoutUrl;
    }
}
