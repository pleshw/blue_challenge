using BlueChallenge.Api.Model.User;
using FluentValidation;

namespace BlueChallenge.Api.Validation;

public class UserModelValidator : AbstractValidator<UserModel>
{
    public UserModelValidator()
    {
        RuleFor(user => user.Id)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(user => user.Credentials)
            .NotNull().WithMessage("Credentials are required.")
            .SetValidator(new UserCredentialsModelValidator());

        When(user => user.PersonalInfo is not null, () =>
        {
            RuleFor(user => user.PersonalInfo!)
                .SetValidator(new UserPersonalInfoModelValidator());
        });
    }
}
