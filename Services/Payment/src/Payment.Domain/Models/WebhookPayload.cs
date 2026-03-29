namespace Payment.Domain.Models;

public class WebhookPayload
{
    public string Action { get; set; }
    public string ApiVersion { get; set; }
    public Data Data { get; set; }
    public DateTime DateCreated { get; set; }
    public long Id { get; set; }
    public bool LiveMode { get; set; }
    public string Type { get; set; }
    public string UserId { get; set; }

    protected WebhookPayload() { }

    public WebhookPayload(string action, string apiVersion, Data data, DateTime dateCreated, long id, bool liveMode, string type, string userId)
    {
        Action = action;
        ApiVersion = apiVersion;
        Data = data;
        DateCreated = dateCreated;
        Id = id;
        LiveMode = liveMode;
        Type = type;
        UserId = userId;
    }
}
public class Data
{
    public string Id { get; set; }
}