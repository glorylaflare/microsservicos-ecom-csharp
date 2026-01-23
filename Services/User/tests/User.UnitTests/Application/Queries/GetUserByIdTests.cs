using BuildingBlocks.Infra.ReadModels;
using FluentAssertions;
using Moq;
using User.Application.Interfaces;
using User.Application.Queries;
using User.Application.Queries.Handlers;
using User.Application.Responses;
namespace User.UnitTests.Application.Queries;

public class GetUserByIdTests
{
    private const int id = 1;
    private readonly GetUserByIdQuery _request = new GetUserByIdQuery(id);
    private readonly Mock<IUserReadService> _mockService = new();
    private readonly UserReadModel _userReadModel = new UserReadModel
    {
        Id = 1,
        Username = "testuser",
        Email = "test@emal.com",
        CreatedAt = It.IsAny<DateTime>(),
        UpdatedAt = It.IsAny<DateTime?>()
    };
    [Fact]
    public async Task GetUserByIdQuery_WhenUserExists_ShouldReturnSuccess()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockService
            .Setup(s => s.GetByIdAsync(id))
            .ReturnsAsync(_userReadModel);
        var response = new GetUserResponse(
            _userReadModel.Id,
            _userReadModel.Username,
            _userReadModel.Email,
            _userReadModel.Status,
            _userReadModel.CreatedAt,
            _userReadModel.UpdatedAt
        );
        var handler = new GetUserByIdQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(response);
    }
    [Fact]
    public async Task GetUserByIdQuery_WhenUserDoesNotExist_ShouldReturnFailure()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockService
            .Setup(s => s.GetByIdAsync(id))
            .ReturnsAsync((UserReadModel?)null);
        var handler = new GetUserByIdQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}