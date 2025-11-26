using BlueChallenge.Api.Contracts.Auth;
using BlueChallenge.Api.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueChallenge.Api.Controller;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;

    public AuthController(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest("Request body is required.");
        }

        var result = await _authenticationService.AuthenticateAsync(request.Email, request.Password, cancellationToken);
        if (result is null)
        {
            return Unauthorized();
        }

        var response = new LoginResponse
        {
            AccessToken = result.AccessToken,
            ExpiresAtUtc = result.ExpiresAtUtc
        };

        return Ok(response);
    }
}
