namespace BlueChallenge.Api.Model.User;

public record class PhoneModel
{
    public required string DDD { get; init; }
    public required string Number { get; init; }
}