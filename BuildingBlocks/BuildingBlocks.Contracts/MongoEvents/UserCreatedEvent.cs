using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.MongoEvents;

public record UserCreatedEvent : IntegrationEvent<UserCreatedData>
{
    [JsonConstructor]
    public UserCreatedEvent(UserCreatedData data)
        : base(data) { }
}
