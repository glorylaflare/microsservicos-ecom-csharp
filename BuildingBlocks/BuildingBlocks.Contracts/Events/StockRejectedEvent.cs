using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events;

public record StockRejectedEvent : IntegrationEvent
{
    public int OrderId { get; init; }
    public string Reason { get; init; }

    public StockRejectedEvent(int orderId, string reason, string correlationId)
        : base(correlationId)
    {
        OrderId = orderId;
        Reason = reason;
    }
}