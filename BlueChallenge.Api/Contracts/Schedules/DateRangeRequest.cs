namespace BlueChallenge.Api.Contracts.Schedules;

public record class DateRangeRequest
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}
