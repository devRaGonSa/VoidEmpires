using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Identity;
using VoidEmpires.Infrastructure;

namespace VoidEmpires.Tests;

public class AccountLoginEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task LoginReturnsCurrentPlayerSummary()
    {
        using var configuredFactory = CreateConfiguredFactory();
        using var client = configuredFactory.CreateClient();
        await RegisterAsync(client);
        using var response = await client.PostAsJsonAsync("/api/accounts/login", ValidLogin());
        var rawJson = await response.Content.ReadAsStringAsync();
        var payload = await response.Content.ReadFromJsonAsync<AccountSessionResult>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("password", rawJson, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.NotNull(payload.UserId);
        Assert.NotNull(payload.PlayerProfileId);
        Assert.NotNull(payload.CivilizationId);
        Assert.NotNull(payload.HomePlanetId);
        Assert.Equal("Nova Prime", payload.HomePlanetName);
        Assert.Equal($"/planet?civilizationId={payload.CivilizationId}&planetId={payload.HomePlanetId}", payload.NextRoute);
    }

    [Theory]
    [InlineData("player@example.test", "WrongP@ssw0rd!23")]
    [InlineData("missing@example.test", "P@ssw0rd!23")]
    public async Task LoginReturnsSafeFailureForInvalidCredentials(string email, string password)
    {
        using var configuredFactory = CreateConfiguredFactory();
        using var client = configuredFactory.CreateClient();
        await RegisterAsync(client);
        using var response = await client.PostAsJsonAsync("/api/accounts/login", new AccountLoginRequest(email, password));
        var rawJson = await response.Content.ReadAsStringAsync();
        var payload = await response.Content.ReadFromJsonAsync<AccountSessionResult>();
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.DoesNotContain(password, rawJson, StringComparison.Ordinal);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains(payload.Errors, error => error.Code == "InvalidCredentials" && error.Field is null);
    }

    [Fact]
    public async Task LoginReturnsSafeValidationErrors()
    {
        using var configuredFactory = CreateConfiguredFactory();
        using var client = configuredFactory.CreateClient();
        using var response = await client.PostAsJsonAsync("/api/accounts/login", new AccountLoginRequest("", ""));
        var payload = await response.Content.ReadFromJsonAsync<AccountSessionResult>();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains(payload.Errors, error => error.Code == "EmailRequired" && error.Field == "email");
        Assert.Contains(payload.Errors, error => error.Code == "PasswordRequired" && error.Field == "password");
    }

    private static async Task RegisterAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync("/api/accounts/register", new AccountRegistrationRequest("player@example.test", "P@ssw0rd!23", "P@ssw0rd!23", "Commander Vega", "Solar Dominion", "Nova Prime"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static AccountLoginRequest ValidLogin() => new("player@example.test", "P@ssw0rd!23");

    private WebApplicationFactory<Program> CreateConfiguredFactory() =>
        factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"))
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddVoidEmpiresIdentity()));
}
