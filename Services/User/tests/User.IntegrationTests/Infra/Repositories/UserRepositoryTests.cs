using FluentAssertions;
using User.Infra.Data.Repositories;
using User.IntegrationTests.Fixture;

namespace User.IntegrationTests.Infra.Repositories;

[Collection("Database Collection")]
public class UserRepositoryTests
{
    private readonly DatabaseFixture _fixture;
    private Domain.Models.User _user = new (
        "Auth0|1234567890",
        "testuser",
        "test@email.com"
    );
    private readonly UserRepository _repository;

    public UserRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new UserRepository(_fixture._context);
    }

    [Fact]
    public async Task AddAsync_WhenValid_ShouldCreateUser()
    {
        //Act
        await _repository.AddAsync(_user);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(_user.Id);
        //Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(_user.Email);
    }
}
