namespace BlueChallenge.Api.Configuration;

public class JwtOptions
{
    public string Issuer { get; set; } = "BlueChallenge.Api";
    public string Audience { get; set; } = "BlueChallenge.Client";
    public string SigningKey { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; } = 60;
}
