using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events;

public record OrderRequestedEvent : IntegrationEvent<OrderRequestedData>
{
    public OrderRequestedEvent(int orderId, List<OrderItemDto> items) 
        : base(new OrderRequestedData(orderId, items)) { }   
}