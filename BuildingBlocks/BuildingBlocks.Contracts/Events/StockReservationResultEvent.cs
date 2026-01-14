using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Events;

public record class StockReservationResultEvent : IntegrationEvent<StockReservationResultData>
{
    [JsonConstructor]
    public StockReservationResultEvent(StockReservationResultData data) 
        : base(data) { }
}
