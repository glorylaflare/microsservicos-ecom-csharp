using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.MongoEvents;

public record OrderCreatedEvent : IntegrationEvent<OrderCreatedData>
{
    [JsonConstructor]
    public OrderCreatedEvent(OrderCreatedData data) 
        : base(data) { }
}
