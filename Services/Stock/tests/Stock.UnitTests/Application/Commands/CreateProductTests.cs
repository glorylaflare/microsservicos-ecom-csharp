using FluentAssertions;
using Moq;
using Stock.Application.Commands.CreateProduct;
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
        var cancellationToken = CancellationToken.None;
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepo
            .Setup(r => r.AddAsync(It.IsAny<Product>(), cancellationToken))
            .Returns(Task.CompletedTask);
        _mockRepo
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);
        var handler = new CreateProductCommandHandler(_mockRepo.Object, _mockValidator.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
    }
    [Fact]
    public async Task CreateProduct_WithInvalidData_ShouldReturnValidationErrors()
    {
        //Arrange
        var cancellationToken = CancellationToken.None;
        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Name", "Name cannot be empty")
        };
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));
        var handler = new CreateProductCommandHandler(_mockRepo.Object, _mockValidator.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}