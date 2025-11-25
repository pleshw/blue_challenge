using BlueChallenge.Api.Contracts.Users;
using FluentValidation;

namespace BlueChallenge.Api.Validation.Requests
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(request => request.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is invalid.")
                .MaximumLength(256).WithMessage("Email must be 256 characters or fewer.");

            RuleFor(request => request.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must have at least 6 characters.")
                .MaximumLength(128).WithMessage("Password must be 128 characters or fewer.");
        }
    }
}
