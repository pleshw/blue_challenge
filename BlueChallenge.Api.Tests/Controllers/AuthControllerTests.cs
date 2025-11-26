using System.Net;
using System.Net.Http.Json;
using BlueChallenge.Api.Contracts.Auth;
using BlueChallenge.Api.Tests.Infrastructure;
using FluentAssertions;

namespace BlueChallenge.Api.Tests.Controllers;

[Collection("Integration Tests")]
public class AuthControllerTests : IntegrationTestBase
{
    public AuthControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var password = "Secret123!";
        var user = await CreateUserAsync(email: "login-success@blue.com", password: password);

        var response = await Client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = user.Credentials.Email.Address,
            Password = password
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
        payload.Should().NotBeNull();
        payload!.AccessToken.Should().NotBeNullOrWhiteSpace();
        payload.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var user = await CreateUserAsync(email: "login-fail@blue.com", password: "Secret123!");

        var response = await Client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = user.Credentials.Email.Address,
            Password = "wrong-password"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
