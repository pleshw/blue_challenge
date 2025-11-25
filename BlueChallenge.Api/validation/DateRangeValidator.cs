using BlueChallenge.Api.Model.Utils;
using FluentValidation;

namespace BlueChallenge.Api.Validation;

public class DateRangeValidator : AbstractValidator<DateRange>
{
    public DateRangeValidator()
    {
        RuleFor(range => range.Start)
            .LessThanOrEqualTo(range => range.End)
            .WithMessage("Start date must be earlier than or equal to end date.");

        RuleFor(range => range.End)
            .GreaterThanOrEqualTo(range => range.Start)
            .WithMessage("End date must be equal to or later than start date.");
    }
}
