using FluentAssertions;
using FluentValidation;
using Moq;
using User.Application.Commands;
using User.Application.Commands.Handlers;
using User.Domain.Interfaces;

namespace User.UnitTests.Application.Commands;

public class DeactivateUserTests
{
    private readonly CreateUserCommand _user = new CreateUserCommand(
        "testuser",
        "testuser@example.com",
        "password123"
    );

    private readonly Mock<IUserRepository> _mockRepo = new();
    private readonly Mock<IValidator<DeactivateUserCommand>> _mockValidator = new();

    [Fact]
    public async Task DeactivateUser_ShouldDeactivate_WhenValidRequest()
    {
        //Arrange
        var user = new User.Domain.Models.User(
                "auth0|1234567890",
                _user.Username,
                _user.Email
            );

        var _request = new DeactivateUserCommand(_user.Email);

        var _cancellationToken = It.IsAny<CancellationToken>();
        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepo
            .Setup(r => r.GetByEmailAsync(_user.Email))
            .ReturnsAsync(user);
        _mockRepo
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        //Act
        var handler = new DeactivateUserCommandHandler(_mockRepo.Object, _mockValidator.Object);

        user.Deactivate();
        var result = await handler.Handle(new DeactivateUserCommand(_user.Email), _cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
        user.Status.Should().Be(User.Domain.Models.Status.Inactive);
    }

    [Fact]
    public async Task DeactivateUser_ShouldReturnFailure_WhenUserNotFound()
    {
        //Arrange
        var _cancellationToken = It.IsAny<CancellationToken>();

        var _request = new DeactivateUserCommand(_user.Email);

        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockRepo
            .Setup(r => r.GetByEmailAsync(_user.Email))
            .ReturnsAsync((User.Domain.Models.User?)null);
        //Act
        var handler = new DeactivateUserCommandHandler(_mockRepo.Object, _mockValidator.Object);
        var result = await handler.Handle(new DeactivateUserCommand(_user.Email), _cancellationToken);
        //Assert
        result.IsFailed.Should().BeTrue();
    }
}
