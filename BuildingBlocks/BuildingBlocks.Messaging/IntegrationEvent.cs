namespace BuildingBlocks.Messaging;

public abstract record IntegrationEvent<TData> : IntegrationEventBase
{
    public TData Data { get; init; }
    protected IntegrationEvent(TData data)
    {
        Data = data;
    }
}
public abstract record IntegrationEventBase
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
    public string Source => GetType().Namespace ?? string.Empty;
}