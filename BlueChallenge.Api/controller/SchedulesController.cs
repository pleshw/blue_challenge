using BlueChallenge.Api.Contracts.Schedules;
using BlueChallenge.Api.Model.Schedule;
using BlueChallenge.Api.Model.User;
using BlueChallenge.Api.Model.Utils;
using BlueChallenge.Api.Repository;
using BlueChallenge.Api.Service;
using BlueChallenge.Api.Service.Telemetry;
using FluentValidation;
using FluentValidationResult = FluentValidation.Results.ValidationResult;
using Microsoft.AspNetCore.Mvc;

namespace BlueChallenge.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUserRepository _userRepository;
    private readonly ScheduleService _scheduleService;
    private readonly IValidator<ScheduleModel> _scheduleValidator;
    private readonly IValidator<CreateScheduleRequest> _createRequestValidator;
    private readonly IValidator<UpdateScheduleRequest> _updateRequestValidator;
    private readonly ITelemetryProducer _telemetryProducer;

    public SchedulesController(
        IScheduleRepository scheduleRepository,
        IUserRepository userRepository,
        ScheduleService scheduleService,
        IValidator<ScheduleModel> scheduleValidator,
        IValidator<CreateScheduleRequest> createRequestValidator,
        IValidator<UpdateScheduleRequest> updateRequestValidator,
        ITelemetryProducer telemetryProducer)
    {
        _scheduleRepository = scheduleRepository;
        _userRepository = userRepository;
        _scheduleService = scheduleService;
        _scheduleValidator = scheduleValidator;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
        _telemetryProducer = telemetryProducer;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleModel>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetAllAsync(cancellationToken);
        return Ok(schedules);
    }

    [HttpGet("{id:guid}", Name = "GetScheduleById")]
    public async Task<ActionResult<ScheduleModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id, cancellationToken);
        if (schedule is null)
        {
            return NotFound();
        }

        return Ok(schedule);
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleModel>> CreateAsync([FromBody] CreateScheduleRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest("Request body is required.");
        }

        FluentValidationResult requestValidation = await _createRequestValidator.ValidateAsync(request, cancellationToken);
        if (!requestValidation.IsValid)
        {
            AddValidationErrors(requestValidation);
            return ValidationProblem(ModelState);
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return NotFound($"User with id {request.UserId} was not found.");
        }

        DateRange dateRange = new(request.DateRange!.Start, request.DateRange.End);
        HourRange? hourRange = request.HourRange is not null
            ? new HourRange(request.HourRange.Start, request.HourRange.End)
            : null;

        ScheduleModel schedule;

        try
        {
            schedule = _scheduleService.CreateSchedule(dateRange, request.IsAllDay, hourRange, request.Description, user);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(CreateScheduleRequest.HourRange), ex.Message);
            return ValidationProblem(ModelState);
        }

        FluentValidationResult validationResult = await _scheduleValidator.ValidateAsync(schedule, cancellationToken);
        if (!validationResult.IsValid)
        {
            AddValidationErrors(validationResult);
            return ValidationProblem(ModelState);
        }

        await _scheduleRepository.CreateAsync(schedule, cancellationToken);
        await _telemetryProducer.PublishAsync("ScheduleCreated", BuildSchedulePayload(schedule), cancellationToken);

        return CreatedAtRoute("GetScheduleById", new { id = schedule.Id }, schedule);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ScheduleModel>> UpdateAsync(Guid id, [FromBody] UpdateScheduleRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest("Request body is required.");
        }

        FluentValidationResult requestValidation = await _updateRequestValidator.ValidateAsync(request, cancellationToken);
        if (!requestValidation.IsValid)
        {
            AddValidationErrors(requestValidation);
            return ValidationProblem(ModelState);
        }

        var schedule = await _scheduleRepository.GetByIdAsync(id, cancellationToken);
        if (schedule is null)
        {
            return NotFound();
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return NotFound($"User with id {request.UserId} was not found.");
        }

        DateRange dateRange = new(request.DateRange!.Start, request.DateRange.End);
        HourRange? hourRange = request.HourRange is not null
            ? new HourRange(request.HourRange.Start, request.HourRange.End)
            : null;

        ScheduleModel updatedSchedule;

        try
        {
            updatedSchedule = _scheduleService.UpdateSchedule(schedule, dateRange, request.IsAllDay, hourRange, request.Description, user);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(UpdateScheduleRequest.HourRange), ex.Message);
            return ValidationProblem(ModelState);
        }

        FluentValidationResult validationResult = await _scheduleValidator.ValidateAsync(updatedSchedule, cancellationToken);
        if (!validationResult.IsValid)
        {
            AddValidationErrors(validationResult);
            return ValidationProblem(ModelState);
        }

        await _scheduleRepository.UpdateAsync(updatedSchedule, cancellationToken);
        await _telemetryProducer.PublishAsync("ScheduleUpdated", BuildSchedulePayload(updatedSchedule), cancellationToken);

        return Ok(updatedSchedule);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _scheduleRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        await _telemetryProducer.PublishAsync("ScheduleDeleted", new { ScheduleId = id }, cancellationToken);

        return NoContent();
    }

    private void AddValidationErrors(FluentValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }

    private static object BuildSchedulePayload(ScheduleModel schedule)
    {
        return new
        {
            schedule.Id,
            schedule.Description,
            schedule.IsAllDay,
            DateRange = new
            {
                schedule.DateRange.Start,
                schedule.DateRange.End
            },
            HourRange = schedule.HourRange is null ? null : new
            {
                schedule.HourRange.Start,
                schedule.HourRange.End
            },
            UserId = schedule.User.Id
        };
    }
}
