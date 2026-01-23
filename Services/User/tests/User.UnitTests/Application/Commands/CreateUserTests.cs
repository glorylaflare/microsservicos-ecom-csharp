using Auth0.AuthenticationApi.Models;
using FluentAssertions;
using Moq;
using User.Application.Commands;
using User.Application.Commands.Handlers;
using User.Application.Interfaces;
using User.Domain.Interfaces;
namespace User.UnitTests.Application.Commands;
public class CreateUserTests
{
    private readonly CreateUserCommand _request = new CreateUserCommand(
        "testuser",
        "testuser@example.com",
        "password123"
    );
    private readonly SignupUserResponse _response = new SignupUserResponse
    {
        Id = "auth0|1234567890"
    };
    private readonly Mock<IUserRepository> _mockRepo = new();
    private readonly Mock<FluentValidation.IValidator<CreateUserCommand>> _mockValidator = new();
    private readonly Mock<IAuthService> _mockAuth = new();
    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnUserId()
    {
        //Arrange
        var user = new User.Domain.Models.User(
            _response.Id,
            _request.Username,
            _request.Email
        );
        var _cancellationToken = It.IsAny<CancellationToken>();
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockAuth
            .Setup(a => a.SignupUserAsync(_request.Email, _request.Password))
            .ReturnsAsync(_response);
        _mockRepo
            .Setup(r => r.AddAsync(user))
            .Returns(Task.CompletedTask);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        var handler = new CreateUserCommandHandler(_mockRepo.Object, _mockValidator.Object, _mockAuth.Object);
        //Act
        var result = await handler.Handle(_request, _cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(user.Id);
    }
    [Fact]
    public async Task CreateUser_WithInvalidData_ShouldReturnValidationErrors()
    {
        //Arrange
        var _cancellationToken = It.IsAny<CancellationToken>();
        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Username", "Username cannot be empty")
        };
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));
        var handler = new CreateUserCommandHandler(_mockRepo.Object, _mockValidator.Object, _mockAuth.Object);
        //Act
        var result = await handler.Handle(_request, _cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}