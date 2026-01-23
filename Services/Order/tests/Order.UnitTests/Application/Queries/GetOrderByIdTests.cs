using BuildingBlocks.Infra.ReadModels;
using FluentAssertions;
using Moq;
using Order.Application.Interfaces;
using Order.Application.Queries;
using Order.Application.Queries.Handlers;
using Order.Application.Responses;
namespace Order.UnitTests.Application.Queries;
public class GetOrderByIdTests
{
    private const int id = 1;
    private readonly GetOrderByIdQuery _request = new GetOrderByIdQuery(id);
    private readonly Mock<IOrderReadService> _mockService = new();
    private readonly OrderReadModel _orderReadModel = new OrderReadModel
    {
        Id = 1,
        Items = It.IsAny<List<OrderItemReadModel>>(),
        TotalAmount = 150m,
        Status = StatusReadModel.Pending,
        CreatedAt = It.IsAny<DateTime>(),
        UpdatedAt = It.IsAny<DateTime?>()
    };
    [Fact]
    public async Task GetOrderByIdQuery_WhenOrderExists_ShouldReturnSuccess()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockService
            .Setup(s => s.GetByIdAsync(id))
            .ReturnsAsync(_orderReadModel);
        var response = new GetOrderResponse(
            _orderReadModel.Id,
            _orderReadModel.Items,
            _orderReadModel.TotalAmount,
            _orderReadModel.Status.ToString(),
            _orderReadModel.CreatedAt,
            _orderReadModel.UpdatedAt
        );
        var handler = new GetOrderByIdQueryHandler(_mockService.Object);
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
        _mockService
            .Setup(s => s.GetByIdAsync(id))
            .ReturnsAsync((OrderReadModel?)null);
        var handler = new GetOrderByIdQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}