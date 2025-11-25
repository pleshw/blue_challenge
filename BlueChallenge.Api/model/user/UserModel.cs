namespace BlueChallenge.Api.Model.User;

public record class UserModel
{
    public Guid Id { get; init; }
    public required UserCredentialsModel Credentials { get; init; }
    public UserPersonalInfoModel? PersonalInfo { get; init; }
}
