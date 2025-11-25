using BlueChallenge.Api.Model.User;
using FluentValidation;

namespace BlueChallenge.Api.Validation;

public class EmailModelValidator : AbstractValidator<EmailModel>
{
    public EmailModelValidator()
    {
        RuleFor(email => email.Alias)
            .NotEmpty().WithMessage("Email alias is required.")
            .Matches("^[A-Za-z0-9._%+-]+$").WithMessage("Email alias contains invalid characters.");

        RuleFor(email => email.Provider)
            .NotEmpty().WithMessage("Email provider is required.")
            .Matches("^[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$").WithMessage("Email provider must be a valid domain.");
    }
}
