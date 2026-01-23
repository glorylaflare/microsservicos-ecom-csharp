using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;
namespace BuildingBlocks.Contracts.Events;

public record OrderRequestedEvent : IntegrationEvent<OrderRequestedData>
{
    [JsonConstructor]
    public OrderRequestedEvent(OrderRequestedData data)
        : base(data) { }
}