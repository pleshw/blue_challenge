using BlueChallenge.Api.Model.User;
using BlueChallenge.Api.Repository;

namespace BlueChallenge.Api.Service.Auth;

public class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthenticationService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthenticationResult?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        EmailModel? parsedEmail = TryParseEmail(email);
        if (parsedEmail is null)
        {
            return null;
        }

        var user = await _userRepository.GetByEmailAsync(parsedEmail.Alias, parsedEmail.Provider, cancellationToken);
        if (user is null)
        {
            return null;
        }

        if (!string.Equals(user.Credentials.Password, password, StringComparison.Ordinal))
        {
            return null;
        }

        return _tokenService.GenerateToken(user);
    }

    private static EmailModel? TryParseEmail(string fullEmail)
    {
        var parts = fullEmail.Split('@', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return null;
        }

        try
        {
            return new EmailModel
            {
                Alias = parts[0],
                Provider = parts[1]
            };
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
