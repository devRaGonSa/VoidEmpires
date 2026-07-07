using System.Text.Json;
using VoidEmpires.Application.Identity;

namespace VoidEmpires.Tests;

public class AccountRegistrationContractTests
{
    private static readonly JsonSerializerOptions WebJsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public void RequestCarriesRegistrationAndInitialWorldFields()
    {
        var request = new AccountRegistrationRequest(
            "player@example.test",
            "P@ssw0rd!23",
            "P@ssw0rd!23",
            "Commander Vega",
            "Solar Dominion",
            "Nova Prime");

        var json = JsonSerializer.Serialize(request, WebJsonOptions);

        Assert.Contains("\"email\"", json);
        Assert.Contains("\"password\"", json);
        Assert.Contains("\"confirmPassword\"", json);
        Assert.Contains("\"displayName\"", json);
        Assert.Contains("\"civilizationName\"", json);
        Assert.Contains("\"homePlanetName\"", json);
    }

    [Fact]
    public void SuccessResponseCarriesSafeBootstrapFieldsOnly()
    {
        var result = AccountRegistrationResult.Success(
            "user-123",
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Guid.Parse("20000000-0000-0000-0000-000000000001"),
            Guid.Parse("30000000-0000-0000-0000-000000000001"),
            "Nova Prime",
            "/planet?civilizationId=20000000-0000-0000-0000-000000000001&planetId=30000000-0000-0000-0000-000000000001");

        var json = JsonSerializer.Serialize(result, WebJsonOptions);

        Assert.Contains("\"succeeded\":true", json);
        Assert.Contains("\"userId\"", json);
        Assert.Contains("\"playerProfileId\"", json);
        Assert.Contains("\"civilizationId\"", json);
        Assert.Contains("\"homePlanetId\"", json);
        Assert.Contains("\"homePlanetName\"", json);
        Assert.Contains("\"nextRoute\"", json);
        Assert.DoesNotContain("password", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("confirmPassword", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FailureResponseUsesStructuredErrorsWithoutIdentityDetails()
    {
        var result = AccountRegistrationResult.Failure(
            new AccountRegistrationError("PasswordMismatch", "Passwords must match.", "confirmPassword"),
            new AccountRegistrationError("CivilizationNameTaken", "Civilization name is already in use.", "civilizationName"));

        var json = JsonSerializer.Serialize(result, WebJsonOptions);

        Assert.False(result.Succeeded);
        Assert.Null(result.UserId);
        Assert.Equal(["PasswordMismatch", "CivilizationNameTaken"], result.Errors.Select(error => error.Code));
        Assert.Contains("\"errors\"", json);
        Assert.Contains("\"field\":\"confirmPassword\"", json);
        Assert.DoesNotContain("PasswordHash", json);
        Assert.DoesNotContain("SecurityStamp", json);
    }
}
