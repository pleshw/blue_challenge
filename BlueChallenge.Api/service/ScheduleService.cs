using BlueChallenge.Api.Model.Schedule;
using BlueChallenge.Api.Model.User;
using BlueChallenge.Api.Model.Utils;
using BlueChallenge.Api.Repository;
using BlueChallenge.Api.Service.Telemetry;

namespace BlueChallenge.Api.Service;

public class ScheduleService
{
    private ITelemetryProducer TelemetryProducer { get; }
    private IScheduleRepository ScheduleRepository { get; }

    public ScheduleService(IScheduleRepository scheduleRepository, ITelemetryProducer telemetryProducer)
    {
        ScheduleRepository = scheduleRepository;
        TelemetryProducer = telemetryProducer;
    }

    public async Task<ScheduleModel> CreateAndSaveScheduleAsync(DateRange dateRange, bool isAllDay, HourRange? hourRange, string description, UserModel user)
    {
        ScheduleModel schedule = CreateSchedule(dateRange, isAllDay, hourRange, description, user);
        await ScheduleRepository.CreateAsync(schedule);
        return schedule;
    }

    public async Task<ScheduleModel> SaveScheduleAsync(ScheduleModel schedule)
    {
        await ScheduleRepository.CreateAsync(schedule);
        return schedule;
    }

    public ScheduleModel CreateSchedule(DateRange dateRange, bool isAllDay, HourRange? hourRange, string description, UserModel user)
    {
        ValidateHourRange(isAllDay, hourRange);

        return new ScheduleModel
        {
            Id = Guid.NewGuid(),
            DateRange = dateRange,
            IsAllDay = isAllDay,
            HourRange = hourRange,
            Description = description,
            User = user
        };
    }

    public ScheduleModel UpdateSchedule(ScheduleModel existingSchedule, DateRange dateRange, bool isAllDay, HourRange? hourRange, string description, UserModel user)
    {
        ArgumentNullException.ThrowIfNull(existingSchedule);
        ValidateHourRange(isAllDay, hourRange);

        return existingSchedule with
        {
            DateRange = dateRange,
            IsAllDay = isAllDay,
            HourRange = hourRange,
            Description = description,
            User = user
        };
    }

    private void ValidateHourRange(bool isAllDay, HourRange? hourRange)
    {
        if (!isAllDay && hourRange == null)
        {
            _ = TelemetryProducer.PublishAsync("ScheduleServiceError", "Hour range is required when the schedule is not all day.");
            throw new ArgumentException("Hour range is required when the schedule is not all day.", nameof(hourRange));
        }
    }
}