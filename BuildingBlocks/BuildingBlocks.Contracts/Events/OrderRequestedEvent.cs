using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events;

public record OrderRequestedEvent(
    int OrderId, 
    List<OrderItemDto> Items
) : IntegrationEvent;