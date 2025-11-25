using BlueChallenge.Api.Contracts.Users;
using BlueChallenge.Api.Model.User;
using BlueChallenge.Api.Repository;
using BlueChallenge.Api.Service;
using BlueChallenge.Api.Service.Telemetry;
using FluentValidation;
using FluentValidationResult = FluentValidation.Results.ValidationResult;
using Microsoft.AspNetCore.Mvc;

namespace BlueChallenge.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly UserService _userService;
    private readonly IValidator<UserModel> _userValidator;
    private readonly IValidator<CreateUserRequest> _createRequestValidator;
    private readonly IValidator<UpdateUserRequest> _updateRequestValidator;
    private readonly ITelemetryProducer _telemetryProducer;

    public UsersController(
        IUserRepository userRepository,
        UserService userService,
        IValidator<UserModel> userValidator,
        IValidator<CreateUserRequest> createRequestValidator,
        IValidator<UpdateUserRequest> updateRequestValidator,
        ITelemetryProducer telemetryProducer)
    {
        _userRepository = userRepository;
        _userService = userService;
        _userValidator = userValidator;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
        _telemetryProducer = telemetryProducer;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:guid}", Name = "GetUserById")]
    public async Task<ActionResult<UserModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserModel>> CreateAsync([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
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

        UserModel user;

        try
        {
            user = _userService.CreateUser(request.Email, request.Password);
        }
        catch (ArgumentException ex)
        {
            var key = ex.ParamName switch
            {
                "fullEmail" => nameof(CreateUserRequest.Email),
                "password" => nameof(CreateUserRequest.Password),
                _ => string.Empty
            };

            var modelKey = string.IsNullOrWhiteSpace(key) ? nameof(CreateUserRequest) : key;
            ModelState.AddModelError(modelKey, ex.Message);
            return ValidationProblem(ModelState);
        }

        FluentValidationResult validationResult = await _userValidator.ValidateAsync(user, cancellationToken);
        if (!validationResult.IsValid)
        {
            AddValidationErrors(validationResult);
            return ValidationProblem(ModelState);
        }

        await _userRepository.CreateAsync(user, cancellationToken);
        await _telemetryProducer.PublishAsync("UserCreated", new { user.Id, Email = user.Credentials.Email.Address }, cancellationToken);

        return CreatedAtRoute("GetUserById", new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserModel>> UpdateAsync(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
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

        var existingUser = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (existingUser is null)
        {
            return NotFound();
        }

        UserModel updatedUser;

        try
        {
            updatedUser = _userService.UpdateUser(existingUser, request.Email, request.Password);
        }
        catch (ArgumentException ex)
        {
            var key = ex.ParamName switch
            {
                "fullEmail" => nameof(UpdateUserRequest.Email),
                "password" => nameof(UpdateUserRequest.Password),
                _ => string.Empty
            };

            var modelKey = string.IsNullOrWhiteSpace(key) ? nameof(UpdateUserRequest) : key;
            ModelState.AddModelError(modelKey, ex.Message);
            return ValidationProblem(ModelState);
        }

        FluentValidationResult validationResult = await _userValidator.ValidateAsync(updatedUser, cancellationToken);
        if (!validationResult.IsValid)
        {
            AddValidationErrors(validationResult);
            return ValidationProblem(ModelState);
        }

        await _userRepository.UpdateAsync(updatedUser, cancellationToken);
        await _telemetryProducer.PublishAsync("UserUpdated", new { updatedUser.Id, Email = updatedUser.Credentials.Email.Address }, cancellationToken);

        return Ok(updatedUser);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _userRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        await _telemetryProducer.PublishAsync("UserDeleted", new { UserId = id }, cancellationToken);

        return NoContent();
    }

    private void AddValidationErrors(FluentValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}
