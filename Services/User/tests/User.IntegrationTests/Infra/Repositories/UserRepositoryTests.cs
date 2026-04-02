using FluentAssertions;
using BuildingBlocks.Infra.Specifications;
using User.Infra.Data.Repositories;
using User.IntegrationTests.Fixture;
namespace User.IntegrationTests.Infra.Repositories;

[Collection("Database Collection")]
public class UserRepositoryTests
{
    private sealed class UserByIdSpec : Specification<Domain.Models.User>
    {
        public UserByIdSpec(int id)
        {
            AddCriteria(x => x.Id == id);
            EnableTracking();
        }
    }

    private sealed class UserByEmailSpec : Specification<Domain.Models.User>
    {
        public UserByEmailSpec(string email)
        {
            AddCriteria(x => x.Email == email);
            EnableTracking();
        }
    }

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
        var cancellationToken = CancellationToken.None;

        //Act
        await _repository.AddAsync(user, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        var result = await _repository.FindOneAsync(new UserByIdSpec(user.Id), cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenValid_ShouldReturnUser()
    {
        //Arrange
        var user = CreateUser();
        var cancellationToken = CancellationToken.None;

        await _repository.AddAsync(user, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        //Act
        var result = await _repository.FindOneAsync(new UserByEmailSpec(user.Email), cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
    }
}