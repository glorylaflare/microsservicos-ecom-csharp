using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.MongoEvents;

public record OrderUpdatedEvent : IntegrationEvent<OrderUpdatedData>
{
    [JsonConstructor]
    public OrderUpdatedEvent(OrderUpdatedData data)
        : base(data) { }
}

