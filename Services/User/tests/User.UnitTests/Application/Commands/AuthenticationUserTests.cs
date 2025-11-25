using Auth0.AuthenticationApi.Models;
using FluentAssertions;
using Moq;
using User.Application.Commands;
using User.Application.Commands.Handlers;
using User.Application.Interfaces;
using User.Application.Responses;

namespace User.UnitTests.Application.Commands;

public class AuthenticationUserTests
{
    private readonly AuthenticateUserCommand _request = new AuthenticateUserCommand(
        "testuser@example.com",
        "password123"
    );

    private readonly AccessTokenResponse _tokenResponse = new AccessTokenResponse
    {
        AccessToken = "Access_Token",
        IdToken = "Id_Token",
        ExpiresIn = 3600
    };

    private readonly Mock<FluentValidation.IValidator<AuthenticateUserCommand>> _mockValidator = new();
    private readonly Mock<IAuthService> _mockAuth = new();

    [Fact]
    public async Task AuthenticateUser_WithValidData_ShouldReturnTokenResponse()
    {
        //Arrange
        var _cancellationToken = It.IsAny<CancellationToken>();
        _mockValidator
            .Setup(x => x.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockAuth
            .Setup(x => x.GetTokenAsync(_request.Email, _request.Password))
            .ReturnsAsync(_tokenResponse);

        var response = new TokenResponse(
            _tokenResponse.AccessToken, 
            _tokenResponse.IdToken, 
            _tokenResponse.ExpiresIn
        );

        var handler = new AuthenticationUserCommandHandler(_mockValidator.Object, _mockAuth.Object);
        //Act
        var result = await handler.Handle(_request, _cancellationToken);
        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(response);
    }
}
