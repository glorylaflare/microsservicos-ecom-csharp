using FluentAssertions;
using Moq;
using Order.Application.Commands;
using Order.Application.Commands.Handlers;
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
    [Fact]
    public async Task CreateOrder_WithValidItems_ShouldReturnOrderId()
    {
        //Arrange
        var order = new Order.Domain.Models.Order(_request.Items);
        var _cancellationToken = It.IsAny<CancellationToken>();
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepo
            .Setup(r => r.AddAsync(order))
            .Returns(Task.CompletedTask);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        var handler = new CreateOrderCommandHandler(_mockRepo.Object, _mockValidator.Object, _mockEventBus.Object);
        //Act
        var result = await handler.Handle(_request, _cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
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
        var handler = new CreateOrderCommandHandler(_mockRepo.Object, _mockValidator.Object, _mockEventBus.Object);
        //Act
        var result = await handler.Handle(_request, _cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}