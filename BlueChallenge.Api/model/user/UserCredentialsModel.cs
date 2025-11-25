namespace BlueChallenge.Api.Model.User;

public record class UserCredentialsModel
{
    public required EmailModel Email { get; init; }
    public required string Password { get; init; }
}
