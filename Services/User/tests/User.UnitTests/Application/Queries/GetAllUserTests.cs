using BuildingBlocks.Infra.ReadModels;
using FluentAssertions;
using Moq;
using User.Application.Interfaces;
using User.Application.Queries;
using User.Application.Queries.Handlers;
using User.Application.Responses;
namespace User.UnitTests.Application.Queries;

public class GetAllUserTests
{
    private readonly GetAllUsersQuery _request = new GetAllUsersQuery();
    private readonly Mock<IUserReadService> _mockService = new();
    private readonly List<UserReadModel> _userReadModels = new List<UserReadModel>
    {
        new UserReadModel
        {
            Id = 1,
            Username = "user1",
            Email = "user1@example.com",
            CreatedAt = It.IsAny<DateTime>(),
            UpdatedAt = It.IsAny<DateTime?>()
        },
        new UserReadModel
        {
            Id = 2,
            Username = "user2",
            Email = "user2@example.com",
            CreatedAt = It.IsAny<DateTime>(),
            UpdatedAt = It.IsAny<DateTime?>()
        }
    };
    [Fact]
    public async Task GetAllUsersQuery_WhenListExists_ShouldReturnSuccess()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockService
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(_userReadModels);
        var response = _userReadModels.Select(u => new GetUserResponse(
            u.Id,
            u.Username,
            u.Email,
            u.Status,
            u.CreatedAt,
            u.UpdatedAt
        ));
        var handler = new GetAllUsersQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(response);
    }
    [Fact]
    public async Task GetAllUsersQuery_WhenListDoesNotExist_ShouldReturnEmptyList()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockService
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<UserReadModel>());
        var handler = new GetAllUsersQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}