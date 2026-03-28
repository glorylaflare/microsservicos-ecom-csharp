using BuildingBlocks.Contracts;
using FluentAssertions;
using MediatR;
using Moq;
using Stock.Application.Commands.OrderFailed;
using Stock.Application.Interfaces;
using Stock.Domain.Interfaces;
using Stock.Domain.Models;

namespace Stock.UnitTests.Application.Commands;

public class OrderFailedTests
{
    private readonly Mock<IProductRepository> _mockRepo = new();
    private readonly Mock<IDbTransactionManager> _mockTransactionManager = new();

    [Fact]
    public async Task OrderFailed_WhenProductsExist_ShouldRestoreStock()
    {
        //Arrange
        var command = new OrderFailedCommand(new List<OrderItemDto>
        {
            new OrderItemDto(1, 2),
            new OrderItemDto(2, 1)
        });
        var cancellationToken = It.IsAny<CancellationToken>();

        var product1 = new Product("Mouse", "Mouse Gamer", 120m, 3);
        var product2 = new Product("Teclado", "Teclado Mecânico", 250m, 5);

        _mockTransactionManager
            .Setup(t => t.ExecuteResilientTransactionAsync(It.IsAny<Func<Task>>()))
            .Returns<Func<Task>>(operation => operation());
        _mockRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product1);
        _mockRepo
            .Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(product2);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var handler = new OrderFailedCommandHandler(_mockRepo.Object, _mockTransactionManager.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        product1.StockQuantity.Should().Be(5);
        product2.StockQuantity.Should().Be(6);
        _mockRepo.Verify(r => r.Update(product1), Times.Once);
        _mockRepo.Verify(r => r.Update(product2), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task OrderFailed_WhenTransactionThrows_ShouldPropagateException()
    {
        //Arrange
        var command = new OrderFailedCommand(new List<OrderItemDto>
        {
            new OrderItemDto(1, 2)
        });
        var cancellationToken = It.IsAny<CancellationToken>();

        _mockTransactionManager
            .Setup(t => t.ExecuteResilientTransactionAsync(It.IsAny<Func<Task>>()))
            .ThrowsAsync(new Exception("transaction error"));

        var handler = new OrderFailedCommandHandler(_mockRepo.Object, _mockTransactionManager.Object);

        //Act
        Func<Task> action = async () => await handler.Handle(command, cancellationToken);

        //Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("transaction error");
    }
}
