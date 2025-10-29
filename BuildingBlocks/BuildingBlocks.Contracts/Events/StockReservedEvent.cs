using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events;

public record StockReservedEvent(
    int OrderId, 
    List<OrderItemDto> Items, 
    decimal TotalAmount
) : IntegrationEvent;
