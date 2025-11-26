using System;
using System.Text.RegularExpressions;

namespace BlueChallenge.Api.Model.User;

public record class PhoneModel
{
    private const string DddPattern = "^\\d{2}$";
    private const string NumberPattern = "^\\d{8,9}$";

    private string _ddd = string.Empty;
    private string _number = string.Empty;

    public required string DDD
    {
        get => _ddd;
        init => _ddd = ValidateDdd(value);
    }

    public required string Number
    {
        get => _number;
        init => _number = ValidateNumber(value);
    }

    private static string ValidateDdd(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("DDD is required.", nameof(value));
        }

        if (!Regex.IsMatch(value, DddPattern, RegexOptions.Compiled))
        {
            throw new ArgumentException("DDD must contain exactly 2 digits.", nameof(value));
        }

        return value;
    }

    private static string ValidateNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Phone number is required.", nameof(value));
        }

        if (!Regex.IsMatch(value, NumberPattern, RegexOptions.Compiled))
        {
            throw new ArgumentException("Phone number must contain 8 or 9 digits.", nameof(value));
        }

        return value;
    }
}