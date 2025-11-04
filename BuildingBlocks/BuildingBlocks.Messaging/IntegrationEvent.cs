using System.Diagnostics;

namespace BuildingBlocks.Messaging;

public abstract record IntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string? CorrelationId { get; init; }
    
    protected IntegrationEvent(string correlationId)
    {
        CorrelationId = correlationId ?? Activity.Current?.TraceId.ToString();
    }
}
