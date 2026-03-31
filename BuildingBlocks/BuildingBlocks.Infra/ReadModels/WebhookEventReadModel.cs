namespace BuildingBlocks.Infra.ReadModels;

public class WebhookEventReadModel
{
    public string Provider { get; init; }
    public string EventId { get; init; }
    public string Action { get; init; }
    public string ApiVersion { get; init; }
    public string PaymentId { get; init; }
    public DateTime DateCreated { get; init; }
    public long ExternalId { get; init; }
    public bool LiveMode { get; init; }
    public string Type { get; init; }
    public string UserId { get; init; }
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
