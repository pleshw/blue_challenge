using BlueChallenge.Api.Model.User;
using FluentValidation;

namespace BlueChallenge.Api.Validation;

public class UserCredentialsModelValidator : AbstractValidator<UserCredentialsModel>
{
    public UserCredentialsModelValidator()
    {
        RuleFor(credentials => credentials.Email)
            .NotNull().WithMessage("Username is required.")
            .SetValidator(new EmailModelValidator());

        RuleFor(credentials => credentials.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(100).WithMessage("Password must be 100 characters or fewer.");
    }
}
