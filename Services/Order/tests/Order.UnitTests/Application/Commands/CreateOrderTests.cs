using BuildingBlocks.Security.Context;
using FluentAssertions;
using Moq;
using Order.Application.Commands;
using Order.Application.Commands.Handlers;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Contracts.MongoEvents;
using Order.Domain.Interfaces;
using Order.Domain.Models;
namespace Order.UnitTests.Application.Commands;

public class CreateOrderTests
{
    private readonly CreateOrderCommand _request = new CreateOrderCommand(new List<OrderItem>
    {
        new OrderItem(1, 2),
        new OrderItem(2, 3)
    });
    
    private readonly Mock<IOrderRepository> _mockRepo = new();
    private readonly Mock<FluentValidation.IValidator<CreateOrderCommand>> _mockValidator = new();
    private readonly Mock<BuildingBlocks.Messaging.IEventBus> _mockEventBus = new();
    private readonly Mock<IUserContext> _mockUserContext = new();

    [Fact]
    public async Task CreateOrder_WithValidItems_ShouldReturnOrderId()
    {
        //Arrange
        var _cancellationToken = It.IsAny<CancellationToken>();
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockUserContext    
            .Setup(u => u.IsAuthenticated)
            .Returns(true);
        _mockUserContext
            .Setup(u => u.UserId)
            .Returns("1");
        _mockRepo
            .Setup(r => r.AddAsync(It.IsAny<Order.Domain.Models.Order>()))
            .Returns(Task.CompletedTask);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        var handler = new CreateOrderCommandHandler(_mockRepo.Object, _mockValidator.Object, _mockEventBus.Object, _mockUserContext.Object);
        //Act
        var result = await handler.Handle(_request, _cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Order.Domain.Models.Order>()), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderCreatedEvent>()), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderRequestedEvent>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidItems_ShouldReturnValidationErrors()
    {
        //Arrange
        var _cancellationToken = It.IsAny<CancellationToken>();
        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Items", "Items cannot be empty")
        };
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));
        _mockUserContext
            .Setup(u => u.IsAuthenticated)
            .Returns(true);
        var handler = new CreateOrderCommandHandler(_mockRepo.Object, _mockValidator.Object, _mockEventBus.Object, _mockUserContext.Object);
        //Act
        var result = await handler.Handle(_request, _cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Order.Domain.Models.Order>()), Times.Never);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderCreatedEvent>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrder_WhenUserIsUnauthorized_ShouldReturnFailure()
    {
        //Arrange
        var _cancellationToken = It.IsAny<CancellationToken>();
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockUserContext
            .Setup(u => u.IsAuthenticated)
            .Returns(false);

        var handler = new CreateOrderCommandHandler(_mockRepo.Object, _mockValidator.Object, _mockEventBus.Object, _mockUserContext.Object);

        //Act
        var result = await handler.Handle(_request, _cancellationToken);

        //Assert
        result.IsFailed.Should().BeTrue();
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Order.Domain.Models.Order>()), Times.Never);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<OrderCreatedEvent>()), Times.Never);
    }
}