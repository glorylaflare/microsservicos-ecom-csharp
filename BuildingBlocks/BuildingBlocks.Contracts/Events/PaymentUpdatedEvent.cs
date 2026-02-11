using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;
namespace BuildingBlocks.Contracts.Events;

public record PaymentUpdatedEvent : IntegrationEvent<PaymentUpdatedData>
{
    [JsonConstructor]
    public PaymentUpdatedEvent(PaymentUpdatedData data)
        : base(data) { }
}