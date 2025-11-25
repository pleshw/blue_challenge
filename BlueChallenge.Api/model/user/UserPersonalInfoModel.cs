namespace BlueChallenge.Api.Model.User;

public record class UserPersonalInfoModel
{
    public required string Name { get; init; }
    public required PhoneModel Phone { get; init; }
    public required List<EmailModel> Emails { get; init; }
}
