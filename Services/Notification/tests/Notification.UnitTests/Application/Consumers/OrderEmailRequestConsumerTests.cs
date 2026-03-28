using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using MediatR;
using Moq;
using Notification.Application.Commands.OrderCompleted;
using Notification.Application.Commands.OrderFailed;
using Notification.Application.Commands.OrderPending;
using Notification.Application.Consumers;

namespace Notification.UnitTests.Application.Consumers;

public class OrderEmailRequestConsumerTests
{
    private readonly Mock<IMediator> _mockMediator = new();

    [Fact]
    public async Task HandleAsync_WhenStatusIsFailed_ShouldSendOrderFailedCommand()
    {
        //Arrange
        var items = new List<OrderItemDto> { new(1, 2) };
        var evt = new OrderEmailRequestEvent(OrderEmailRequestData.Failed("user@email.com", 10, items, "payment rejected"));

        _mockMediator
            .Setup(m => m.Send(It.IsAny<OrderFailedEmailCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var consumer = new OrderEmailRequestConsumer(_mockMediator.Object);

        //Act
        await consumer.HandleAsync(evt);

        //Assert
        _mockMediator.Verify(m => m.Send(
            It.Is<OrderFailedEmailCommand>(c =>
                c.Email == "user@email.com" &&
                c.OrderId == 10 &&
                c.Reason == "payment rejected" &&
                c.Items.Count == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenStatusIsPending_ShouldSendOrderPendingCommand()
    {
        //Arrange
        var items = new List<OrderItemDto> { new(1, 2) };
        var evt = new OrderEmailRequestEvent(OrderEmailRequestData.Pending("user@email.com", 20, items, "https://checkout/20"));

        _mockMediator
            .Setup(m => m.Send(It.IsAny<OrderPendingEmailCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var consumer = new OrderEmailRequestConsumer(_mockMediator.Object);

        //Act
        await consumer.HandleAsync(evt);

        //Assert
        _mockMediator.Verify(m => m.Send(
            It.Is<OrderPendingEmailCommand>(c =>
                c.Email == "user@email.com" &&
                c.OrderId == 20 &&
                c.CheckoutUrl == "https://checkout/20" &&
                c.Items.Count == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenStatusIsCompleted_ShouldSendOrderCompletedCommand()
    {
        //Arrange
        var items = new List<OrderItemDto> { new(1, 2), new(2, 3) };
        var evt = new OrderEmailRequestEvent(OrderEmailRequestData.Completed("user@email.com", 30, items));

        _mockMediator
            .Setup(m => m.Send(It.IsAny<OrderCompletedEmailCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var consumer = new OrderEmailRequestConsumer(_mockMediator.Object);

        //Act
        await consumer.HandleAsync(evt);

        //Assert
        _mockMediator.Verify(m => m.Send(
            It.Is<OrderCompletedEmailCommand>(c =>
                c.Email == "user@email.com" &&
                c.OrderId == 30 &&
                c.Items.Count == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenStatusIsUnknown_ShouldNotSendAnyCommand()
    {
        //Arrange
        var items = new List<OrderItemDto> { new(1, 1) };
        var evt = new OrderEmailRequestEvent(new OrderEmailRequestData((OrderPaymentStatus)999, "user@email.com", 40, items, null, null));

        var consumer = new OrderEmailRequestConsumer(_mockMediator.Object);

        //Act
        await consumer.HandleAsync(evt);

        //Assert
        _mockMediator.Verify(m => m.Send(It.IsAny<IRequest<Unit>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
