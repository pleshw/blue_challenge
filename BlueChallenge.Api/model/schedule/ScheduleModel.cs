using BlueChallenge.Api.Model.Utils;
using BlueChallenge.Api.Model.User;

namespace BlueChallenge.Api.Model.Schedule;

public record class ScheduleModel
{
    public Guid Id { get; init; }
    public required DateRange DateRange { get; init; }
    public required bool IsAllDay { get; init; }
    public HourRange? HourRange { get; init; }
    public required string Description { get; init; }
    public required UserModel User { get; init; }
}
