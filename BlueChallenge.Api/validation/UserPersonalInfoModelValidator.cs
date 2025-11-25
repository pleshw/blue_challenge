using BlueChallenge.Api.Model.User;
using FluentValidation;

namespace BlueChallenge.Api.Validation;

public class UserPersonalInfoModelValidator : AbstractValidator<UserPersonalInfoModel>
{
    public UserPersonalInfoModelValidator()
    {
        RuleFor(info => info.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(120).WithMessage("Name must be 120 characters or fewer.");

        RuleFor(info => info.Phone)
            .NotNull().WithMessage("Phone is required.")
            .SetValidator(new PhoneModelValidator());
    }
}
