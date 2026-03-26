using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Events;

public record OrderEmailRequestEvent : IntegrationEvent<OrderEmailRequestData>
{
    [JsonConstructor]
    public OrderEmailRequestEvent(OrderEmailRequestData data)
        : base(data) { }
}
