using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events;

public record StockReservedEvent : IntegrationEvent
{
    public int OrderId { get; init; }
    public List<OrderItemDto> Items { get; init; }
    public decimal TotalAmount { get; init; }

    public StockReservedEvent(int orderId, List<OrderItemDto> items, decimal totalAmount, string correlationId) : base(correlationId)
    {
        OrderId = orderId;
        Items = items;
        TotalAmount = totalAmount;
    }
}