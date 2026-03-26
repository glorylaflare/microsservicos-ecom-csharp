using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Messaging;
using FluentAssertions;
using MediatR;
using Moq;
using Order.Application.Commands;
using Order.Application.Commands.Handlers;
using Order.Application.Interfaces;
using Order.Domain.Interfaces;
using Order.Domain.Models;

namespace Order.UnitTests.Application.Commands;

public class PaymentUpdatedCommandTests
{
    private readonly Mock<IOrderRepository> _mockRepo = new();
    private readonly Mock<IEventBus> _mockEventBus = new();
    private readonly Mock<IOrderEmailPublisher> _mockOrderEmailPublisher = new();

    [Fact]
    public async Task PaymentUpdated_WhenStatusIsPaid_ShouldCompleteOrder()
    {
        //Arrange
        var command = new PaymentUpdatedCommand(1, "paid", "http://checkout-url");
        var cancellationToken = It.IsAny<CancellationToken>();
        var order = new Order.Domain.Models.Order("user-1", new List<OrderItem> { new OrderItem(1, 2) });

        _mockRepo
            .Setup(r => r.GetByIdAsync(command.OrderId))
            .ReturnsAsync(order);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var handler = new PaymentUpdatedCommandHandler(_mockRepo.Object, _mockEventBus.Object, _mockOrderEmailPublisher.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        order.Status.Should().Be(Status.Completed);
        _mockRepo.Verify(r => r.Update(order), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderUpdatedEvent>()), Times.Once);
        _mockOrderEmailPublisher.Verify(e => e.PublishCompleted(order), Times.Once);
        _mockOrderEmailPublisher.Verify(e => e.PublishFailed(It.IsAny<Order.Domain.Models.Order>()), Times.Never);
        _mockOrderEmailPublisher.Verify(e => e.PublishPending(It.IsAny<Order.Domain.Models.Order>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PaymentUpdated_WhenStatusIsFailed_ShouldCancelOrderAndPublishFailedEvent()
    {
        //Arrange
        var command = new PaymentUpdatedCommand(1, "failed", "http://checkout-url");
        var cancellationToken = It.IsAny<CancellationToken>();
        var order = new Order.Domain.Models.Order("user-1", new List<OrderItem> { new OrderItem(1, 2) });

        _mockRepo
            .Setup(r => r.GetByIdAsync(command.OrderId))
            .ReturnsAsync(order);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        var handler = new PaymentUpdatedCommandHandler(_mockRepo.Object, _mockEventBus.Object, _mockOrderEmailPublisher.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        order.Status.Should().Be(Status.Cancelled);
        _mockRepo.Verify(r => r.Update(order), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderUpdatedEvent>()), Times.Once);
        _mockOrderEmailPublisher.Verify(e => e.PublishFailed(order), Times.Once);
        _mockOrderEmailPublisher.Verify(e => e.PublishCompleted(It.IsAny<Order.Domain.Models.Order>()), Times.Never);
        _mockOrderEmailPublisher.Verify(e => e.PublishPending(It.IsAny<Order.Domain.Models.Order>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PaymentUpdated_WhenOrderDoesNotExist_ShouldReturnUnitValue()
    {
        //Arrange
        var command = new PaymentUpdatedCommand(999, "paid", "http://checkout-url");
        var cancellationToken = It.IsAny<CancellationToken>();

        _mockRepo
            .Setup(r => r.GetByIdAsync(command.OrderId))
            .ReturnsAsync((Order.Domain.Models.Order?)null);

        var handler = new PaymentUpdatedCommandHandler(_mockRepo.Object, _mockEventBus.Object, _mockOrderEmailPublisher.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        _mockRepo.Verify(r => r.Update(It.IsAny<Order.Domain.Models.Order>()), Times.Never);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderUpdatedEvent>()), Times.Never);
        _mockOrderEmailPublisher.Verify(e => e.PublishCompleted(It.IsAny<Order.Domain.Models.Order>()), Times.Never);
        _mockOrderEmailPublisher.Verify(e => e.PublishFailed(It.IsAny<Order.Domain.Models.Order>()), Times.Never);
        _mockOrderEmailPublisher.Verify(e => e.PublishPending(It.IsAny<Order.Domain.Models.Order>(), It.IsAny<string>()), Times.Never);
    }
}
