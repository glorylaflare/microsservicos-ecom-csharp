using BuildingBlocks.Infra.ReadModels;
using FluentAssertions;
using Moq;
using Stock.Application.Interfaces;
using Stock.Application.Queries;
using Stock.Application.Queries.Handlers;
using Stock.Application.Responses;
namespace Stock.UnitTests.Application.Queries;

public class GetProductByIdTests
{
    private const int id = 1;
    private readonly GetProductByIdQuery _request = new GetProductByIdQuery(id);
    private readonly Mock<IProductReadService> _mockService = new();
    private readonly ProductReadModel _productReadModel = new ProductReadModel
    {
        Id = 1,
        Name = "Test Product",
        Description = "This is a test product",
        Price = 99.99m,
        StockQuantity = 50,
        CreatedAt = It.IsAny<DateTime>(),
        UpdatedAt = It.IsAny<DateTime?>()
    };
    [Fact]
    public async Task GetProductByIdQuery_WhenProductExists_ShouldReturnSuccess()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockService
            .Setup(s => s.GetByIdAsync(id))
            .ReturnsAsync(_productReadModel);
        var response = new GetProductResponse(
            _productReadModel.Id,
            _productReadModel.Name,
            _productReadModel.Description,
            _productReadModel.Price,
            _productReadModel.StockQuantity,
            _productReadModel.CreatedAt,
            _productReadModel.UpdatedAt
        );
        var handler = new GetProductByIdQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(response);
    }
    [Fact]
    public async Task GetProductByIdQuery_WhenProductDoesNotExist_ShouldReturnFailure()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockService
            .Setup(s => s.GetByIdAsync(id))
            .ReturnsAsync((ProductReadModel?)null);
        var handler = new GetProductByIdQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}