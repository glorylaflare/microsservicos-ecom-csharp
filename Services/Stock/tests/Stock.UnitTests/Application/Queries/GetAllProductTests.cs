using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.Infra.Interfaces;
using FluentAssertions;
using Moq;
using Stock.Application.Interfaces;
using Stock.Application.Queries.GetAllProducts;
using Stock.Application.Responses;
namespace Stock.UnitTests.Application.Queries;

public class GetAllProductTests
{
    private readonly GetAllProductsQuery _request = new GetAllProductsQuery(0, 10);
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
        var cancellationToken = CancellationToken.None;
        _mockService
            .Setup(s => s.WhereAsync(It.IsAny<ISpecification<ProductReadModel, ProductReadModel>>(), cancellationToken))
            .ReturnsAsync(_productReadModels);
        var response = _productReadModels.Select(p => new ProductResponse(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.StockQuantity,
            p.CreatedAt,
            p.UpdatedAt
        ));
        var handler = new GetAllProductsQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEquivalentTo(response);
    }
    [Fact]
    public async Task GetAllProductsQuery_WhenNoProductsExist_ShouldReturnFailure()
    {
        //Arrange
        var cancellationToken = CancellationToken.None;
        _mockService
            .Setup(s => s.WhereAsync(It.IsAny<ISpecification<ProductReadModel, ProductReadModel>>(), cancellationToken))
            .ReturnsAsync(new List<ProductReadModel>());
        var handler = new GetAllProductsQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}