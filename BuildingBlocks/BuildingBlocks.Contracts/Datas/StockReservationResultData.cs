namespace BuildingBlocks.Contracts.Datas;

public record StockReservationResultData(
    int OrderId,
    bool IsReserved,
    List<ProductItemDto>? Items,
    decimal TotalAmount,
    string? Reason   
);
