using BlueChallenge.Api.Contracts.Auth;
using FluentValidation;

namespace BlueChallenge.Api.Validation.Requests;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be valid.");

        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
