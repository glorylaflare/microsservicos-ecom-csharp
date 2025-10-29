using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events;

public record StockRejectedEvent(
    int OrderId,
    string Reason
) : IntegrationEvent;
