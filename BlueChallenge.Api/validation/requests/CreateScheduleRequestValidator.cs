using BlueChallenge.Api.Contracts.Schedules;
using FluentValidation;

namespace BlueChallenge.Api.Validation.Requests
{
    public class CreateScheduleRequestValidator : AbstractValidator<CreateScheduleRequest>
    {
        public CreateScheduleRequestValidator()
        {
            RuleFor(request => request.DateRange)
                .NotNull().WithMessage("Date range is required.")
                .SetValidator(new DateRangeRequestValidator());

            RuleFor(request => request.UserId)
                .NotEmpty().WithMessage("User id is required.");

            RuleFor(request => request.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(512).WithMessage("Description must be 512 characters or fewer.");

            When(request => request.IsAllDay, () =>
            {
                RuleFor(request => request.HourRange)
                    .NotNull().WithMessage("Hour range is required when the schedule is all day.");

                When(request => request.HourRange is not null, () =>
                {
                    RuleFor(request => request.HourRange!)
                        .SetValidator(new HourRangeRequestValidator());
                });
            });

            When(request => !request.IsAllDay && request.HourRange is not null, () =>
            {
                RuleFor(request => request.HourRange!)
                    .SetValidator(new HourRangeRequestValidator());
            });
        }
    }
}
