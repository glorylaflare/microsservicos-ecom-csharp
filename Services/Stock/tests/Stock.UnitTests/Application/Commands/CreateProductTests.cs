using FluentAssertions;
using Moq;
using Stock.Application.Commands;
using Stock.Application.Commands.Handlers;
using Stock.Domain.Interfaces;
using Stock.Domain.Models;

namespace Stock.UnitTests.Application.Commands;

public class CreateProductTests
{
    private readonly CreateProductCommand _request = new CreateProductCommand(
        "Test Product",
        "This is a test product",
        99.99m,
        10
    );

    private readonly Mock<IProductRepository> _mockRepo = new();
    private readonly Mock<FluentValidation.IValidator<CreateProductCommand>> _mockValidator = new();

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldReturnProductId()
    {
        //Arrange
        var product = new Product(
            _request.Name, 
            _request.Description, 
            _request.Price, 
            _request.StockQuantity
        );
        var _cancellationToken = It.IsAny<CancellationToken>();
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepo
            .Setup(r => r.AddAsync(product))
            .Returns(Task.CompletedTask);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        var handler = new CreateProductCommandHandler(_mockRepo.Object, _mockValidator.Object);
        //Act
        var result = await handler.Handle(_request, _cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ShouldReturnValidationErrors()
    {
        //Arrange
        var _cancellationToken = It.IsAny<CancellationToken>();
        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Name", "Name cannot be empty")
        };
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));
        var handler = new CreateProductCommandHandler(_mockRepo.Object, _mockValidator.Object);
        //Act
        var result = await handler.Handle(_request, _cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}
