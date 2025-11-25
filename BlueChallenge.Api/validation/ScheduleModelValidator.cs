using BlueChallenge.Api.Model.Schedule;
using FluentValidation;

namespace BlueChallenge.Api.Validation;

public class ScheduleModelValidator : AbstractValidator<ScheduleModel>
{
    public ScheduleModelValidator()
    {
        RuleFor(schedule => schedule.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(512).WithMessage("Description must be 512 characters or fewer.");

        RuleFor(schedule => schedule.DateRange)
            .NotNull().WithMessage("Date range is required.")
            .SetValidator(new DateRangeValidator());

        RuleFor(schedule => schedule.User)
            .NotNull().WithMessage("User is required.")
            .SetValidator(new UserModelValidator());

        RuleFor(schedule => schedule.HourRange)
            .NotNull().WithMessage("Hour range is required when the schedule is all day.")
            .When(schedule => schedule.IsAllDay);
    }
}
