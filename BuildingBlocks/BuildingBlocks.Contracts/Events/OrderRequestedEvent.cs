using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events;

public record OrderRequestedEvent : IntegrationEvent
{
    public int OrderId { get; init; }
    public List<OrderItemDto> Items { get; init; }

    public OrderRequestedEvent(int orderId, List<OrderItemDto> items, string correlationId)
        : base(correlationId)
    {
        OrderId = orderId;
        Items = items;
    }
}