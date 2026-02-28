using FluentAssertions;
using User.Infra.Data.Repositories;
using User.IntegrationTests.Fixture;
namespace User.IntegrationTests.Infra.Repositories;

[Collection("Database Collection")]
public class UserRepositoryTests
{
    private readonly DatabaseFixture _fixture;
    private readonly UserRepository _repository;

    private static Domain.Models.User CreateUser()
    {
        var suffix = Random.Shared.Next(10000, 99999);

        return new Domain.Models.User(
            auth0UserId: $"auth0|{suffix}",
            username: $"testuser{suffix}",
            email: $"test{suffix}@email.com"
        );
    }

    public UserRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new UserRepository(_fixture._context);
    }

    [Fact]
    public async Task AddAsync_WhenValid_ShouldCreateUser()
    {
        //Arrange
        var user = CreateUser();

        //Act
        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(user.Id);

        //Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenValid_ShouldReturnUser()
    {
        //Arrange
        var user = CreateUser();

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        //Act
        var result = await _repository.GetByEmailAsync(user.Email);

        //Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
    }
}