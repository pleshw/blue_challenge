using BlueChallenge.Api.Contracts.Schedules;
using FluentValidation;

namespace BlueChallenge.Api.Validation.Requests
{
    public class HourRangeRequestValidator : AbstractValidator<HourRangeRequest>
    {
        public HourRangeRequestValidator()
        {
            RuleFor(range => range.Start)
                .NotEqual(default(TimeSpan)).WithMessage("Start time is required.");

            RuleFor(range => range.End)
                .NotEqual(default(TimeSpan)).WithMessage("End time is required.");

            RuleFor(range => range)
                .Must(range => range.Start <= range.End)
                .WithMessage("Start time must be earlier than or equal to end time.");
        }
    }
}
