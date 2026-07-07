using VoidEmpires.Application.Identity;

namespace VoidEmpires.Tests;

public class AccountRegistrationValidationTests
{
    [Fact]
    public void ValidInputReturnsNormalizedRequest()
    {
        var result = AccountRegistrationValidator.Validate(new AccountRegistrationRequest(
            " PLAYER@Example.TEST ",
            "P@ssw0rd!23",
            "P@ssw0rd!23",
            " Commander   Vega ",
            " Solar   Dominion ",
            " Nova   Prime "));

        Assert.True(result.Succeeded);
        Assert.Empty(result.Errors);
        Assert.Equal("player@example.test", result.NormalizedRequest?.Email);
        Assert.Equal("Commander Vega", result.NormalizedRequest?.DisplayName);
        Assert.Equal("Solar Dominion", result.NormalizedRequest?.CivilizationName);
        Assert.Equal("Nova Prime", result.NormalizedRequest?.HomePlanetName);
    }

    [Fact]
    public void MissingFieldsReturnStructuredErrors()
    {
        var result = AccountRegistrationValidator.Validate(new AccountRegistrationRequest("", "", "", " ", " "));

        Assert.False(result.Succeeded);
        Assert.Null(result.NormalizedRequest);
        AssertError(result, "EmailRequired", "email");
        AssertError(result, "PasswordRequired", "password");
        AssertError(result, "ConfirmPasswordRequired", "confirmPassword");
        AssertError(result, "DisplayNameRequired", "displayName");
        AssertError(result, "CivilizationNameRequired", "civilizationName");
    }

    [Theory]
    [InlineData("invalid-email", "EmailInvalid", "email")]
    [InlineData("weak", "PasswordTooWeak", "password")]
    public void InvalidEmailAndWeakPasswordReturnSafeErrors(string value, string code, string field)
    {
        var request = ValidRequest();
        var result = AccountRegistrationValidator.Validate(field == "email"
            ? request with { Email = value }
            : request with { Password = value, ConfirmPassword = value });

        AssertError(result, code, field);
        Assert.DoesNotContain(value, result.Errors.Single(error => error.Code == code).Message);
    }

    [Fact]
    public void MismatchedConfirmationReturnsConfirmPasswordError()
    {
        var result = AccountRegistrationValidator.Validate(ValidRequest() with { ConfirmPassword = "OtherP@ssw0rd!23" });

        AssertError(result, "PasswordMismatch", "confirmPassword");
        Assert.DoesNotContain("OtherP@ssw0rd", result.Errors.Single().Message);
    }

    [Fact]
    public void NameLengthLimitsAreEnforced()
    {
        var longName = new string('A', AccountRegistrationValidator.MaxNameLength + 1);
        var result = AccountRegistrationValidator.Validate(ValidRequest() with
        {
            DisplayName = longName,
            CivilizationName = longName,
            HomePlanetName = longName
        });

        AssertError(result, "DisplayNameTooLong", "displayName");
        AssertError(result, "CivilizationNameTooLong", "civilizationName");
        AssertError(result, "HomePlanetNameTooLong", "homePlanetName");
    }

    private static AccountRegistrationRequest ValidRequest() => new(
        "player@example.test",
        "P@ssw0rd!23",
        "P@ssw0rd!23",
        "Commander Vega",
        "Solar Dominion",
        "Nova Prime");

    private static void AssertError(AccountRegistrationValidationResult result, string code, string field)
    {
        Assert.Contains(result.Errors, error => error.Code == code && error.Field == field);
    }
}
