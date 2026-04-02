using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Infra.Interfaces;
using BuildingBlocks.Messaging;
using FluentAssertions;
using MediatR;
using Moq;
using Order.Application.Commands.StockRejected;
using Order.Domain.Interfaces;
using Order.Domain.Models;

namespace Order.UnitTests.Application.Commands;

public class StockRejectedCommandTests
{
    private readonly Mock<IOrderRepository> _mockRepo = new();
    private readonly Mock<IEventBus> _mockEventBus = new();

    [Fact]
    public async Task StockRejected_WhenOrderExists_ShouldCancelOrderAndPublishEvent()
    {
        //Arrange
        var command = new StockRejectedCommand(1, "out of stock");
        var cancellationToken = CancellationToken.None;
        var order = new Order.Domain.Models.Order("1", new List<OrderItem> { new OrderItem(1, 2) });

        _mockRepo
            .Setup(r => r.FindOneAsync(It.IsAny<ISpecification<Order.Domain.Models.Order>>(), cancellationToken))
            .ReturnsAsync(order);
        _mockRepo
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        var handler = new StockRejectedCommandHandler(_mockRepo.Object, _mockEventBus.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        order.Status.Should().Be(Status.Cancelled);
        _mockRepo.Verify(r => r.Update(order), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task StockRejected_WhenOrderDoesNotExist_ShouldNotUpdateOrPublish()
    {
        //Arrange
        var command = new StockRejectedCommand(999, "not found");
        var cancellationToken = CancellationToken.None;

        _mockRepo
            .Setup(r => r.FindOneAsync(It.IsAny<ISpecification<Order.Domain.Models.Order>>(), cancellationToken))
            .ReturnsAsync((Order.Domain.Models.Order?)null);

        var handler = new StockRejectedCommandHandler(_mockRepo.Object, _mockEventBus.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        _mockRepo.Verify(r => r.Update(It.IsAny<Order.Domain.Models.Order>()), Times.Never);
        _mockRepo.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderUpdatedEvent>()), Times.Never);
    }
}
