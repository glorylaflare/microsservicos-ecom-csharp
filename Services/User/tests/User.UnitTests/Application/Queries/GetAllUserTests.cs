using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.SharedKernel.Common;
using FluentAssertions;
using Moq;
using User.Application.Interfaces;
using User.Application.Queries.GetAllUsers;
using User.Application.Responses;
using User.Application.Specifications;
namespace User.UnitTests.Application.Queries;

public class GetAllUserTests
{
    private readonly GetAllUsersQuery _request = new GetAllUsersQuery(0, 10);
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
            .Setup(s => s.WhereAsync(It.IsAny<AllUsersSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_userReadModels);
        var response = _userReadModels.Select(u => new UserResponse(
            u.Id,
            u.Username,
            u.Email,
            u.Status,
            u.CreatedAt,
            u.UpdatedAt
        )).ToList();
        var handler = new GetAllUsersQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new PageResult<UserResponse>
        {
            Items = response,
            Total = response.Count,
            Skip = _request.Skip,
            Take = _request.Take
        });
    }
    [Fact]
    public async Task GetAllUsersQuery_WhenListDoesNotExist_ShouldReturnEmptyList()
    {
        //Arrange
        var cancellationToken = It.IsAny<CancellationToken>();
        _mockService
            .Setup(s => s.WhereAsync(It.IsAny<AllUsersSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserReadModel>());
        var handler = new GetAllUsersQueryHandler(_mockService.Object);
        //Act
        var result = await handler.Handle(_request, cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}