using System.Net;
using System.Net.Http.Json;
using BlueChallenge.Api.Contracts.Users;
using BlueChallenge.Api.Model.User;
using BlueChallenge.Api.Tests.Infrastructure;
using FluentAssertions;

namespace BlueChallenge.Api.Tests.Controllers;

[Collection("Integration Tests")]
public class UsersControllerTests : IntegrationTestBase
{
    public UsersControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedAndPublishesTelemetry()
    {
        var request = new CreateUserRequest
        {
            Email = "integration@blue.com",
            Password = "StrongPass123"
        };

        var response = await Client.PostAsJsonAsync("/api/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var user = await response.Content.ReadFromJsonAsync<UserModel>();
        user.Should().NotBeNull();
        user!.Credentials.Email.Address.Should().Be(request.Email);

        Factory.TelemetryProducer.Events
            .Should()
            .Contain(e => e.EventType == "UserCreated");
    }

    [Fact]
    public async Task CreateUser_WithInvalidPayload_ReturnsBadRequest()
    {
        var request = new CreateUserRequest
        {
            Email = string.Empty,
            Password = string.Empty
        };

        var response = await Client.PostAsJsonAsync("/api/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteUser_RemovesUserAndPublishesTelemetry()
    {
        var user = await CreateUserAsync(email: "delete-me@blue.com");

        var response = await Client.DeleteAsync($"/api/users/{user.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        Factory.TelemetryProducer.Events
            .Should()
            .Contain(e => e.EventType == "UserDeleted");
    }
}
