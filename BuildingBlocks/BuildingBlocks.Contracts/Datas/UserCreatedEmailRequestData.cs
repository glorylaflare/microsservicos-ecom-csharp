using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Datas;

public record UserCreatedEmailRequestData
{
    public string Email { get; init; }
    public string Username { get; init; }

    [JsonConstructor]
    public UserCreatedEmailRequestData(string email, string username)
    {  
        Email = email;
        Username = username; 
    }
}
