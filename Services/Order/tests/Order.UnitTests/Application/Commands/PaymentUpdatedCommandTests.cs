using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Infra.MongoReadModels;
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
    private readonly Mock<IUserReadService> _mockUserService = new();

    [Fact]
    public async Task PaymentUpdated_WhenStatusIsPaid_ShouldCompleteOrder()
    {
        //Arrange
        var command = new PaymentUpdatedCommand(1, "paid");
        var cancellationToken = It.IsAny<CancellationToken>();
        var order = new Order.Domain.Models.Order("user-1", new List<OrderItem> { new OrderItem(1, 2) });

        _mockRepo
            .Setup(r => r.GetByIdAsync(command.OrderId))
            .ReturnsAsync(order);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var handler = new PaymentUpdatedCommandHandler(_mockRepo.Object, _mockEventBus.Object, _mockUserService.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        order.Status.Should().Be(Status.Completed);
        _mockRepo.Verify(r => r.Update(order), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderUpdatedEvent>()), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderFailedEvent>()), Times.Never);
    }

    [Fact]
    public async Task PaymentUpdated_WhenStatusIsFailed_ShouldCancelOrderAndPublishFailedEvent()
    {
        //Arrange
        var command = new PaymentUpdatedCommand(1, "failed");
        var cancellationToken = It.IsAny<CancellationToken>();
        var order = new Order.Domain.Models.Order("user-1", new List<OrderItem> { new OrderItem(1, 2) });

        _mockRepo
            .Setup(r => r.GetByIdAsync(command.OrderId))
            .ReturnsAsync(order);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        _mockUserService
            .Setup(u => u.GetByIdAsync(order.UserId))
            .ReturnsAsync(new UserReadModel
            {
                Id = order.UserId,
                Username = "test-user",
                Email = "test@email.com"
            });

        var handler = new PaymentUpdatedCommandHandler(_mockRepo.Object, _mockEventBus.Object, _mockUserService.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        order.Status.Should().Be(Status.Cancelled);
        _mockRepo.Verify(r => r.Update(order), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderUpdatedEvent>()), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderFailedEvent>()), Times.Once);
    }

    [Fact]
    public async Task PaymentUpdated_WhenOrderDoesNotExist_ShouldReturnUnitValue()
    {
        //Arrange
        var command = new PaymentUpdatedCommand(999, "paid");
        var cancellationToken = It.IsAny<CancellationToken>();

        _mockRepo
            .Setup(r => r.GetByIdAsync(command.OrderId))
            .ReturnsAsync((Order.Domain.Models.Order?)null);

        var handler = new PaymentUpdatedCommandHandler(_mockRepo.Object, _mockEventBus.Object, _mockUserService.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        _mockRepo.Verify(r => r.Update(It.IsAny<Order.Domain.Models.Order>()), Times.Never);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderUpdatedEvent>()), Times.Never);
    }
}
