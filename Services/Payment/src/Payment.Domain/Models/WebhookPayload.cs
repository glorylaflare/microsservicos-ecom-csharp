using System.Text.Json.Serialization;

namespace Payment.Domain.Models;

public class WebhookPayload
{
    [JsonPropertyName("action")]
    public string Action { get; set; }

    [JsonPropertyName("api_version")]
    public string? ApiVersion { get; set; }

    [JsonPropertyName("data")]
    public Data Data { get; set; }

    [JsonPropertyName("date_created")]
    public DateTime DateCreated { get; set; }

    [JsonPropertyName("id")]
    public long ExternalId { get; set; }

    [JsonPropertyName("live_mode")]
    public bool LiveMode { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    protected WebhookPayload() { }

    public WebhookPayload(string action, string apiVersion, Data data, DateTime dateCreated, long externalId, bool liveMode, string type, string userId)
    {
        Action = action;
        ApiVersion = apiVersion;
        Data = data;
        DateCreated = dateCreated;
        ExternalId = externalId;
        LiveMode = liveMode;
        Type = type;
        UserId = userId;
    }
}
public class Data
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}