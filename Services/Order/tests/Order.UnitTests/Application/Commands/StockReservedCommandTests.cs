using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Messaging;
using FluentAssertions;
using MediatR;
using Moq;
using Order.Application.Commands.StockReserved;
using Order.Domain.Interfaces;
using Order.Domain.Models;

namespace Order.UnitTests.Application.Commands;

public class StockReservedCommandTests
{
    private readonly Mock<IOrderRepository> _mockRepo = new();
    private readonly Mock<IEventBus> _mockEventBus = new();

    [Fact]
    public async Task StockReserved_WhenOrderExists_ShouldConfirmOrderAndPublishEvent()
    {
        //Arrange
        var command = new StockReservedCommand(1, 120m);
        var cancellationToken = It.IsAny<CancellationToken>();
        var order = new Order.Domain.Models.Order("1", new List<OrderItem> { new OrderItem(1, 2) });

        _mockRepo
            .Setup(r => r.GetByIdAsync(command.OrderId))
            .ReturnsAsync(order);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var handler = new StockReservedCommandHandler(_mockRepo.Object, _mockEventBus.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        order.Status.Should().Be(Status.Reserved);
        order.TotalAmount.Should().Be(command.TotalAmount);
        _mockRepo.Verify(r => r.Update(order), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task StockReserved_WhenOrderDoesNotExist_ShouldNotUpdateOrPublish()
    {
        //Arrange
        var command = new StockReservedCommand(999, 120m);
        var cancellationToken = It.IsAny<CancellationToken>();

        _mockRepo
            .Setup(r => r.GetByIdAsync(command.OrderId))
            .ReturnsAsync((Order.Domain.Models.Order?)null);

        var handler = new StockReservedCommandHandler(_mockRepo.Object, _mockEventBus.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        _mockRepo.Verify(r => r.Update(It.IsAny<Order.Domain.Models.Order>()), Times.Never);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderUpdatedEvent>()), Times.Never);
    }
}
