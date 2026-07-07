using System.Net.Mail;
using System.Text.RegularExpressions;

namespace VoidEmpires.Application.Identity;

public static class AccountRegistrationValidator
{
    public const int MaxNameLength = 128;
    public const int MinPasswordLength = 8;
    public const int RequiredUniquePasswordChars = 1;
    public const bool RequirePasswordDigit = true;
    public const bool RequirePasswordLowercase = true;
    public const bool RequirePasswordUppercase = true;
    public const bool RequirePasswordNonAlphanumeric = true;

    public static AccountRegistrationValidationResult Validate(AccountRegistrationRequest request)
    {
        var errors = new List<AccountRegistrationError>();
        var email = NormalizeEmail(request.Email);
        var displayName = NormalizeName(request.DisplayName);
        var civilizationName = NormalizeName(request.CivilizationName);
        var homePlanetName = NormalizeOptionalName(request.HomePlanetName);

        if (string.IsNullOrWhiteSpace(email)) Add(errors, "EmailRequired", "Email is required.", "email");
        else if (!IsValidEmail(email)) Add(errors, "EmailInvalid", "Email must be valid.", "email");

        if (string.IsNullOrWhiteSpace(request.Password)) Add(errors, "PasswordRequired", "Password is required.", "password");
        else if (!IsStrongPassword(request.Password)) Add(errors, "PasswordTooWeak", "Password does not meet the minimum policy.", "password");

        if (string.IsNullOrWhiteSpace(request.ConfirmPassword)) Add(errors, "ConfirmPasswordRequired", "Password confirmation is required.", "confirmPassword");
        else if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal)) Add(errors, "PasswordMismatch", "Passwords must match.", "confirmPassword");

        ValidateRequiredName(displayName, "DisplayNameRequired", "Display name is required.", "DisplayNameTooLong", "Display name is too long.", "displayName", errors);
        ValidateRequiredName(civilizationName, "CivilizationNameRequired", "Civilization name is required.", "CivilizationNameTooLong", "Civilization name is too long.", "civilizationName", errors);

        if (homePlanetName?.Length > MaxNameLength)
        {
            Add(errors, "HomePlanetNameTooLong", "Home planet name is too long.", "homePlanetName");
        }

        var normalizedRequest = errors.Count == 0
            ? request with
            {
                Email = email,
                DisplayName = displayName,
                CivilizationName = civilizationName,
                HomePlanetName = homePlanetName
            }
            : null;

        return new AccountRegistrationValidationResult(errors.Count == 0, normalizedRequest, errors);
    }

    private static void ValidateRequiredName(
        string value,
        string requiredCode,
        string requiredMessage,
        string tooLongCode,
        string tooLongMessage,
        string field,
        List<AccountRegistrationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Add(errors, requiredCode, requiredMessage, field);
        }
        else if (value.Length > MaxNameLength)
        {
            Add(errors, tooLongCode, tooLongMessage, field);
        }
    }

    private static string NormalizeEmail(string value) => value.Trim().ToLowerInvariant();

    private static string NormalizeName(string value) => Regex.Replace(value.Trim(), @"\s+", " ");

    private static string? NormalizeOptionalName(string? value)
    {
        var normalized = NormalizeName(value ?? string.Empty);
        return normalized.Length == 0 ? null : normalized;
    }

    private static bool IsValidEmail(string value)
    {
        try
        {
            var address = new MailAddress(value);
            return string.Equals(address.Address, value, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool IsStrongPassword(string value) =>
        value.Length >= MinPasswordLength &&
        (!RequirePasswordUppercase || value.Any(char.IsUpper)) &&
        (!RequirePasswordLowercase || value.Any(char.IsLower)) &&
        (!RequirePasswordDigit || value.Any(char.IsDigit)) &&
        (!RequirePasswordNonAlphanumeric || value.Any(ch => !char.IsLetterOrDigit(ch))) &&
        value.Distinct().Count() >= RequiredUniquePasswordChars;

    private static void Add(List<AccountRegistrationError> errors, string code, string message, string field) =>
        errors.Add(new AccountRegistrationError(code, message, field));
}

public sealed record AccountRegistrationValidationResult(
    bool Succeeded,
    AccountRegistrationRequest? NormalizedRequest,
    IReadOnlyList<AccountRegistrationError> Errors);
