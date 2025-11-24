using BuildingBlocks.Infra.ReadModels;
using FluentAssertions;
using Moq;
using Stock.Application.Interfaces;
using Stock.Application.Queries;
using Stock.Application.Queries.Handlers;
using Stock.Application.Responses;

namespace Stock.UnitTests.Application.Queries;

public class GetAllProductTests
{
    private readonly GetAllProductsQuery _request = new GetAllProductsQuery();
    private readonly Mock<IProductReadService> _mockService = new();
    private readonly List<ProductReadModel> _productReadModels = new List<ProductReadModel>
    {
        new ProductReadModel
        {
            Id = 1,
            Name = "Test Product 1",
            Description = "This is a test product 1",
            Price = 99.99m,
            StockQuantity = 50,
            CreatedAt = It.IsAny<DateTime>(),
            UpdatedAt = It.IsAny<DateTime?>()
        },
        new ProductReadModel
        {
            Id = 2,
            Name = "Test Product 2",
            Description = "This is a test product 2",
            Price = 149.99m,
            StockQuantity = 30,
            CreatedAt = It.IsAny<DateTime>(),
            UpdatedAt = It.IsAny<DateTime?>()
        }
    };

    [Fact]
    public async Task GetAllProductsQuery_WhenListExists_ShouldReturnSuccess()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockService
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(_productReadModels);

        var response = _productReadModels.Select(p => new GetProductResponse(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.StockQuantity,
            p.CreatedAt,
            p.UpdatedAt
        ));

        var handler = new GetAllProductQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetAllProductsQuery_WhenNoProductsExist_ShouldReturnFailure()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockService
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<ProductReadModel>());
        var handler = new GetAllProductQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}
