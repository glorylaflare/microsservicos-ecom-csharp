using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record UserCreatedData
{
    public string UserId { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }

    [JsonConstructor]
    public UserCreatedData(string userId, string username, string email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    }
}
