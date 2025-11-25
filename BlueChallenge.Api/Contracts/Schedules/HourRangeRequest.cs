namespace BlueChallenge.Api.Contracts.Schedules;

public record class HourRangeRequest
{
    public TimeSpan Start { get; init; }
    public TimeSpan End { get; init; }
}
