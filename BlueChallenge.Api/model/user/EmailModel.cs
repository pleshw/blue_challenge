namespace BlueChallenge.Api.Model.User;

public record class EmailModel
{
    public required string Alias { get; init; }
    public required string Provider { get; init; }
    public string Address => $"{Alias}@{Provider}";
}