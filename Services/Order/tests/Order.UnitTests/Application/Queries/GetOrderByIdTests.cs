using BuildingBlocks.Infra.MongoReadModels;
using FluentAssertions;
using Moq;
using Order.Application.Interfaces;
using Order.Application.Queries;
using Order.Application.Queries.Handlers;
using Order.Application.Responses;
namespace Order.UnitTests.Application.Queries;

public class GetOrderByIdTests
{
    private const int ID = 1;
    private readonly GetOrderByIdQuery _request = new GetOrderByIdQuery(ID);
    private readonly Mock<IOrderReadService> _mockOrderService = new();
    private readonly Mock<IUserReadService> _mockUserService = new();

    private readonly OrderReadModel _orderReadModel = new OrderReadModel
    {
        Id = 1,
        UserId = It.IsAny<string>(),
        Items = It.IsAny<List<OrderItemReadModel>>(),
        TotalAmount = 150m,
        Status = "Pending",
        CreatedAt = It.IsAny<DateTime>(),
        UpdatedAt = It.IsAny<DateTime?>()
    };

    [Fact]
    public async Task GetOrderByIdQuery_WhenOrderExists_ShouldReturnSuccess()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockOrderService
            .Setup(s => s.GetByIdAsync(ID))
            .ReturnsAsync(_orderReadModel);
        var response = new GetOrderResponse(
            _orderReadModel.Id,
            _orderReadModel.Items,
            _orderReadModel.TotalAmount,
            _orderReadModel.Status.ToString(),
            _orderReadModel.CreatedAt,
            _orderReadModel.UpdatedAt
        );
        var handler = new GetOrderByIdQueryHandler(_mockOrderService.Object, _mockUserService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetOrderByIdQuery_WhenOrderDoesNotExist_ShouldReturnFailure()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockOrderService
            .Setup(s => s.GetByIdAsync(ID))
            .ReturnsAsync((OrderReadModel?)null);
        var handler = new GetOrderByIdQueryHandler(_mockOrderService.Object, _mockUserService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}