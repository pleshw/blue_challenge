using BlueChallenge.Api.Repository;
using BlueChallenge.Api.Service;
using FluentAssertions;
using NSubstitute;

namespace BlueChallenge.Api.Tests.Services;

public class UserServiceTests
{
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService(_repository);
    }

    [Fact]
    public void CreateUser_WithValidInput_ReturnsUserWithCredentials()
    {
        var user = _sut.CreateUser("teste_da_silva@blue.com", "pass123");

        user.Id.Should().NotBe(Guid.Empty);
        user.Credentials.Email.Address.Should().Be("teste_da_silva@blue.com");
        user.Credentials.Password.Should().Be("pass123");
    }

    [Fact]
    public async Task CreateAndSaveUserAsync_PersistsUser()
    {
        var user = await _sut.CreateAndSaveUserAsync("teste_da_silva@blue.com", "secret123");

        await _repository.Received(1).CreateAsync(user, Arg.Any<CancellationToken>());
    }
}
