using System.Net.Http;
using System.Net.Http.Json;
using BlueChallenge.Api.Contracts.Users;
using BlueChallenge.Api.Model.User;
using Xunit;

namespace BlueChallenge.Api.Tests.Infrastructure;

public abstract class IntegrationTestBase : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    protected TestWebApplicationFactory Factory { get; }
    protected HttpClient Client { get; }

    protected IntegrationTestBase(TestWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;

    public virtual Task InitializeAsync() => Factory.ResetAsync();

    protected async Task<UserModel> CreateUserAsync(string? email = null, string? password = null)
    {
        var request = new CreateUserRequest
        {
            Email = email ?? $"tester-{Guid.NewGuid():N}@blue.com",
            Password = password ?? "Secret123!"
        };

        var response = await Client.PostAsJsonAsync("/api/users", request);
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserModel>();
        return user ?? throw new InvalidOperationException("Response did not contain a user payload.");
    }
}
