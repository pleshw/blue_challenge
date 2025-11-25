namespace BlueChallenge.Api.Contracts.Schedules;

public record class CreateScheduleRequest
{
    public DateRangeRequest DateRange { get; init; } = null!;
    public bool IsAllDay { get; init; }
    public HourRangeRequest? HourRange { get; init; }
    public string Description { get; init; } = string.Empty;
    public Guid UserId { get; init; }
}
