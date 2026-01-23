using FluentAssertions;
namespace User.UnitTests.Domain.Models;

public class UserTests
{
    private const string _auth0UserId = "auth0|123456789";
    private const string _username = "testuser";
    private const string _email = "test@email.com";
    [Fact]
    public void CreateUser_WhenValid_ShouldReturnValidParameters()
    {
        // Act
        var user = new User.Domain.Models.User(_auth0UserId, _username, _email);
        // Assert
        user.Username.Should().Be(_username);
    }
}