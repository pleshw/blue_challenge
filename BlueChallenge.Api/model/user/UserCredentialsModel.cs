using System;

namespace BlueChallenge.Api.Model.User;

public record class UserCredentialsModel
{
    private const int MinPasswordLength = 6;
    private const int MaxPasswordLength = 100;
    private const string PasswordParamName = "password";

    private string _password = string.Empty;

    public required EmailModel Email { get; init; }

    public required string Password
    {
        get => _password;
        init => _password = ValidatePassword(value);
    }

    private static string ValidatePassword(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Password is required.", PasswordParamName);
        }

        if (value.Length < MinPasswordLength)
        {
            throw new ArgumentException($"Password must be at least {MinPasswordLength} characters long.", PasswordParamName);
        }

        if (value.Length > MaxPasswordLength)
        {
            throw new ArgumentException($"Password must be {MaxPasswordLength} characters or fewer.", PasswordParamName);
        }

        return value;
    }
}
