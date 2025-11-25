using System.Net;
using System.Net.Http.Json;
using BlueChallenge.Api.Contracts.Schedules;
using BlueChallenge.Api.Model.Schedule;
using BlueChallenge.Api.Tests.Infrastructure;
using FluentAssertions;

namespace BlueChallenge.Api.Tests.Controllers;

public class SchedulesControllerTests : IntegrationTestBase
{
    public SchedulesControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateSchedule_ReturnsCreatedAndPublishesTelemetry()
    {
        var user = await CreateUserAsync("schedule-user@blue.com");
        var request = new CreateScheduleRequest
        {
            DateRange = new DateRangeRequest
            {
                Start = DateTime.UtcNow.Date,
                End = DateTime.UtcNow.Date.AddDays(1)
            },
            Description = "Planning",
            IsAllDay = false,
            HourRange = new HourRangeRequest
            {
                Start = TimeSpan.FromHours(9),
                End = TimeSpan.FromHours(10)
            },
            UserId = user.Id
        };

        var response = await Client.PostAsJsonAsync("/api/schedules", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var schedule = await response.Content.ReadFromJsonAsync<ScheduleModel>();
        schedule.Should().NotBeNull();
        schedule!.Description.Should().Be("Planning");

        Factory.TelemetryProducer.Events
            .Should()
            .Contain(e => e.EventType == "ScheduleCreated");
    }

    [Fact]
    public async Task CreateSchedule_ForUnknownUser_ReturnsNotFound()
    {
        var request = new CreateScheduleRequest
        {
            DateRange = new DateRangeRequest
            {
                Start = DateTime.UtcNow.Date,
                End = DateTime.UtcNow.Date.AddDays(1)
            },
            Description = "Ghost",
            IsAllDay = false,
            UserId = Guid.NewGuid()
        };

        var response = await Client.PostAsJsonAsync("/api/schedules", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
