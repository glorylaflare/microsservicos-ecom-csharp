using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record StockReservationResultData
{
    public int OrderId { get; init; }
    public bool IsReserved { get; init; }
    public List<ProductItemDto>? Items { get; init; } = new();
    public decimal TotalAmount { get; init; }
    public string? Reason { get; init; }

    [JsonConstructor]
    public StockReservationResultData(int orderId, bool isReserved, List<ProductItemDto>? items, decimal totalAmount, string? reason)
    {
        OrderId = orderId;
        IsReserved = isReserved;
        Items = items;
        TotalAmount = totalAmount;
        Reason = reason;
    }

    public static StockReservationResultData Success(int orderId, List<ProductItemDto> items, decimal totalAmount) =>
        new(orderId, true, items, totalAmount, null);

    public static StockReservationResultData Failure(int orderId, string reason) => 
        new(orderId, false, null, 0, reason);
}
