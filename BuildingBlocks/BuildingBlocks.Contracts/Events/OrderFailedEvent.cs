using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Events;

public record OrderFailedEvent : IntegrationEvent<OrderFailedData>
{
    [JsonConstructor]
    public OrderFailedEvent(OrderFailedData data)
        : base(data) { }
}