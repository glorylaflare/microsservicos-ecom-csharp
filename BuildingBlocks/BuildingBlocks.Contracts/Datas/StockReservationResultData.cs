namespace BuildingBlocks.Contracts.Datas;

public record StockReservationResultData(
    int OrderId,
    bool IsReserved,
    List<OrderItemDto>? Items,
    decimal TotalAmount,
    string? Reason   
);
