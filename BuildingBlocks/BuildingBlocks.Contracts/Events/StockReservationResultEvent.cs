using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events;

public record class StockReservationResultEvent : IntegrationEvent<StockReservationResultData>
{
    public StockReservationResultEvent(int orderId, bool isReserved, List<OrderItemDto> items, decimal totalAmount) 
        : base(new StockReservationResultData(orderId, isReserved, items, totalAmount, null)) { }

    public StockReservationResultEvent(int orderId, bool isReserved, string reason) 
        : base(new StockReservationResultData(orderId, isReserved, new (), 0m, reason)) { }
}
