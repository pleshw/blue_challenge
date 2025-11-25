using BlueChallenge.Api.Model.Schedule;
using BlueChallenge.Api.Model.User;
using BlueChallenge.Api.Model.Utils;
using BlueChallenge.Api.Repository;
using BlueChallenge.Api.Service;
using BlueChallenge.Api.Service.Telemetry;
using FluentAssertions;
using NSubstitute;

namespace BlueChallenge.Api.Tests.Services;

public class ScheduleServiceTests
{
    private readonly IScheduleRepository _scheduleRepository = Substitute.For<IScheduleRepository>();
    private readonly ITelemetryProducer _telemetryProducer = Substitute.For<ITelemetryProducer>();
    private readonly ScheduleService _sut;
    private readonly UserModel _user;

    public ScheduleServiceTests()
    {
        _sut = new ScheduleService(_scheduleRepository, _telemetryProducer);
        _user = new UserModel
        {
            Id = Guid.NewGuid(),
            Credentials = new UserCredentialsModel
            {
                Email = new EmailModel
                {
                    Alias = "teste_da_silva",
                    Provider = "blue.com"
                },
                Password = "secret"
            }
        };
    }

    [Fact]
    public async Task CreateSchedule_AllDayWithoutHourRange_ThrowsAndLogsTelemetry()
    {
        var dateRange = new DateRange(DateTime.UtcNow.Date, DateTime.UtcNow.Date);

        var act = () => _sut.CreateSchedule(dateRange, true, null, "standup", _user);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Hour range is required when the schedule is all day.*");

        await _telemetryProducer
            .Received(1)
            .PublishAsync("ScheduleServiceError", Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void CreateSchedule_WithHourRange_ReturnsConfiguredSchedule()
    {
        var dateRange = new DateRange(DateTime.UtcNow.Date, DateTime.UtcNow.Date);
        var hourRange = new HourRange(TimeSpan.FromHours(9), TimeSpan.FromHours(10));

        var schedule = _sut.CreateSchedule(dateRange, false, hourRange, "1:1", _user);

        schedule.Id.Should().NotBe(Guid.Empty);
        schedule.Description.Should().Be("1:1");
        schedule.HourRange.Should().Be(hourRange);
        schedule.User.Should().Be(_user);
    }
}
