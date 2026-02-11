using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record UserCreatedData
{
    public string Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }

    [JsonConstructor]
    public UserCreatedData(string id, string username, string email)
    {
        Id = id;
        Username = username;
        Email = email;
    }
}
