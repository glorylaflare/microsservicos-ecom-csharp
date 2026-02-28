using BuildingBlocks.Contracts;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using FluentAssertions;
using MediatR;
using Moq;
using Stock.Application.Commands;
using Stock.Application.Commands.Handlers;
using Stock.Application.Interfaces;
using Stock.Domain.Interfaces;
using Stock.Domain.Models;

namespace Stock.UnitTests.Application.Commands;

public class OrderRequestTests
{
    private readonly Mock<IProductRepository> _mockRepo = new();
    private readonly Mock<IDbTransactionManager> _mockTransactionManager = new();
    private readonly Mock<IEventBus> _mockEventBus = new();

    [Fact]
    public async Task OrderRequest_WhenStockIsAvailable_ShouldReserveItemsAndPublishSuccessEvent()
    {
        //Arrange
        var command = new OrderRequestCommand(10, new List<OrderItemDto>
        {
            new OrderItemDto(1, 2)
        });
        var cancellationToken = It.IsAny<CancellationToken>();
        var product = new Product("Notebook", "Notebook gamer", 1000m, 10);

        _mockTransactionManager
            .Setup(t => t.ExecuteResilientTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns<Func<Task>>(operation => operation());
        _mockRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var handler = new OrderRequestCommandHandler(_mockRepo.Object, _mockTransactionManager.Object, _mockEventBus.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        product.StockQuantity.Should().Be(8);
        _mockRepo.Verify(r => r.Update(product), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<StockReservationResultEvent>()), Times.Once);
    }

    [Fact]
    public async Task OrderRequest_WhenProductNotFound_ShouldPublishFailureEvent()
    {
        //Arrange
        var command = new OrderRequestCommand(10, new List<OrderItemDto>
        {
            new OrderItemDto(99, 2)
        });
        var cancellationToken = It.IsAny<CancellationToken>();

        _mockTransactionManager
            .Setup(t => t.ExecuteResilientTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns<Func<Task>>(operation => operation());
        _mockRepo
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Product?)null);

        var handler = new OrderRequestCommandHandler(_mockRepo.Object, _mockTransactionManager.Object, _mockEventBus.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<StockReservationResultEvent>()), Times.Once);
    }
}
