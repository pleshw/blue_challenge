using System;
using System.Text.RegularExpressions;

namespace BlueChallenge.Api.Model.User;

public record class EmailModel
{
    private const string AliasPattern = "^[A-Za-z0-9._%+-]+$";
    private const string ProviderPattern = "^[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$";

    private string _alias = string.Empty;
    private string _provider = string.Empty;

    public required string Alias
    {
        get => _alias;
        init => _alias = ValidateAlias(value);
    }

    public required string Provider
    {
        get => _provider;
        init => _provider = ValidateProvider(value);
    }

    public string Address => $"{Alias}@{Provider}";

    private static string ValidateAlias(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email alias is required.", nameof(Alias));
        }

        if (!Regex.IsMatch(value, AliasPattern, RegexOptions.Compiled))
        {
            throw new ArgumentException("Email alias contains invalid characters.", nameof(Alias));
        }

        return value;
    }

    private static string ValidateProvider(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email provider is required.", nameof(Provider));
        }

        if (!Regex.IsMatch(value, ProviderPattern, RegexOptions.Compiled))
        {
            throw new ArgumentException("Email provider must be a valid domain.", nameof(Provider));
        }

        return value;
    }
}