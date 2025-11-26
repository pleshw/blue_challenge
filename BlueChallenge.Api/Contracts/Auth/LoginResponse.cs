namespace BlueChallenge.Api.Contracts.Auth;

public class LoginResponse
{
    public required string AccessToken { get; init; }
    public required DateTime ExpiresAtUtc { get; init; }
}
