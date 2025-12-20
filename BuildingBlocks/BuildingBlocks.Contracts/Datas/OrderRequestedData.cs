namespace BuildingBlocks.Contracts.Datas;

public record OrderRequestedData(    
    int OrderId,
    List<OrderItemDto> Items
);
