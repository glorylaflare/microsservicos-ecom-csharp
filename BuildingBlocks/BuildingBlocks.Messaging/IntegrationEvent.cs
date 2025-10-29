namespace BuildingBlocks.Messaging;

public abstract record IntegrationEvent
{
    public Guid CorrelationId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
