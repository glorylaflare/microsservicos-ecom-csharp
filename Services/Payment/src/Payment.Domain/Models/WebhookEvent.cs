using BuildingBlocks.SharedKernel.Common;

namespace Payment.Domain.Models;

public class WebhookEvent : EntityBase
{
    public string Provider { get; private set; } 
    public string EventId { get; private set; }
    public WebhookPayload Payload { get; private set; }
    public WebhookStatus Status { get; private set; }

    protected WebhookEvent() { }

    public WebhookEvent(string eventId, WebhookPayload payload)
    {
        Provider = "MercadoPago";
        EventId = eventId;
        Payload = payload;
        Status = WebhookStatus.Pending;
    }

    public void MarkAsProcessed()
    {
        Status = WebhookStatus.Processed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = WebhookStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum WebhookStatus
{
    Pending,
    Processed,
    Failed
}