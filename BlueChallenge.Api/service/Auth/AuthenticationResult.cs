namespace BlueChallenge.Api.Service.Auth;

public sealed record AuthenticationResult(string AccessToken, DateTime ExpiresAtUtc);
