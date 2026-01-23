using FluentAssertions;
using Moq;
using Stock.Application.Commands;
using Stock.Domain.Interfaces;
using Stock.Domain.Models;
namespace Stock.UnitTests.Application.Commands;

public class UpdateStockTests
{
    private readonly UpdateStockCommand _request = new(
        1,
        5
    );
    private readonly Mock<IProductRepository> _mockRepo = new();
    private readonly Mock<FluentValidation.IValidator<UpdateStockCommand>> _mockValidator = new();
    [Fact]
    public async Task UpdateStock_WithValidData_ShouldReturnSuccess()
    {
        //Arrange
        var product = new Product(
            "Test Product",
            "This is a test product",
            99.99m,
            10
        );
        var _cancellationToken = It.IsAny<CancellationToken>();
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepo
            .Setup(r => r.GetByIdAsync(_request.ProductId))
            .ReturnsAsync(product);
        _mockRepo
            .Setup(r => r.Update(product));
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        var handler = new Stock.Application.Commands.Handlers.UpdateStockCommandHandler(_mockRepo.Object, _mockValidator.Object);
        //Act
        var result = await handler.Handle(_request, _cancellationToken);
        //Assert
        product.StockQuantity.Should().Be(5);
    }
    [Fact]
    public async Task UpdateStock_WithInvalidData_ShouldReturnValidationErrors()
    {
        //Arrange
        var _cancellationToken = It.IsAny<CancellationToken>();
        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Quantity", "Quantity must be greater than zero")
        };
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));
        var handler = new Stock.Application.Commands.Handlers.UpdateStockCommandHandler(_mockRepo.Object, _mockValidator.Object);
        //Act
        var result = await handler.Handle(_request, _cancellationToken);
        //Assert
        result.Errors.Should().ContainSingle(e => e.Message == "Quantity must be greater than zero");
    }
}