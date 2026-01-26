using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.MongoEvents;

public record UserUpdatedEvent : IntegrationEvent<UserUpdatedData>
{
    [JsonConstructor]
    public UserUpdatedEvent(UserUpdatedData data)
        : base(data) { }
}
