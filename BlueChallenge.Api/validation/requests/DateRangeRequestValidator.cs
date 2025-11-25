using BlueChallenge.Api.Contracts.Schedules;
using FluentValidation;

namespace BlueChallenge.Api.Validation.Requests
{
    public class DateRangeRequestValidator : AbstractValidator<DateRangeRequest>
    {
        public DateRangeRequestValidator()
        {
            RuleFor(range => range.Start)
                .NotEqual(default(DateTime)).WithMessage("Start date is required.");

            RuleFor(range => range.End)
                .NotEqual(default(DateTime)).WithMessage("End date is required.");

            RuleFor(range => range)
                .Must(range => range.Start <= range.End)
                .WithMessage("Start date must be earlier than or equal to end date.");
        }
    }
}
