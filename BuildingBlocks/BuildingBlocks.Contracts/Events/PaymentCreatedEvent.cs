using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;
namespace BuildingBlocks.Contracts.Events;

public record PaymentCreatedEvent : IntegrationEvent<PaymentCreatedData>
{
    [JsonConstructor]
    public PaymentCreatedEvent(PaymentCreatedData data)
        : base(data) { }
}