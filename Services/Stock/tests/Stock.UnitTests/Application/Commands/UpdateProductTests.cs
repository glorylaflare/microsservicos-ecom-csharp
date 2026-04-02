using FluentAssertions;
using BuildingBlocks.Infra.Interfaces;
using Moq;
using Stock.Application.Commands.UpdateProduct;
using Stock.Domain.Interfaces;
using Stock.Domain.Models;
namespace Stock.UnitTests.Application.Commands;

public class UpdateProductTests
{
    private readonly UpdateProductCommand _request = new UpdateProductCommand(
        1,
        "Updated Product",
        "This is an updated test product",
        149.99m
    );
    private readonly Mock<IProductRepository> _mockRepo = new();
    private readonly Mock<FluentValidation.IValidator<UpdateProductCommand>> _mockValidator = new();
    [Fact]
    public async Task UpdateProduct_WithValidData_ShouldReturnSuccess()
    {
        //Arrange
        var product = new Product(
            "Original Product",
            "This is the original product",
            99.99m,
            10
        );
        var cancellationToken = CancellationToken.None;
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepo
            .Setup(r => r.FindOneAsync(It.IsAny<ISpecification<Product>>(), cancellationToken))
            .ReturnsAsync(product);
        _mockRepo
            .Setup(r => r.Update(product));
        _mockRepo
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);
        var handler = new UpdateProductCommandHandler(_mockRepo.Object, _mockValidator.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        product.Price.Should().Be(149.99m);
    }
    [Fact]
    public async Task UpdateProduct_WithInvalidData_ShouldReturnValidationErrors()
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
        var handler = new UpdateProductCommandHandler(_mockRepo.Object, _mockValidator.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}