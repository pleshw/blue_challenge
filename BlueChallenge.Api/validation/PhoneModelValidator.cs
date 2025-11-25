using BlueChallenge.Api.Model.User;
using FluentValidation;

namespace BlueChallenge.Api.Validation;

public class PhoneModelValidator : AbstractValidator<PhoneModel>
{
    public PhoneModelValidator()
    {
        RuleFor(phone => phone.DDD)
            .NotEmpty().WithMessage("DDD is required.")
            .Matches("^\\d{2}$").WithMessage("DDD must contain exactly 2 digits.");

        RuleFor(phone => phone.Number)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches("^\\d{8,9}$").WithMessage("Phone number must contain 8 or 9 digits.");
    }
}
