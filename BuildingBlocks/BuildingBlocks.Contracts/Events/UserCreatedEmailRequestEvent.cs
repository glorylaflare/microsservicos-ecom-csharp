using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Events;

public record UserCreatedEmailRequestEvent : IntegrationEvent<UserCreatedEmailRequestData>
{
    [JsonConstructor]
    public UserCreatedEmailRequestEvent(UserCreatedEmailRequestData data)
        : base(data) { }
}