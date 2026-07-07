using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Identity;
using VoidEmpires.Application.Players;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class AccountRegistrationEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task RegisterCreatesAccountAndInitialWorld()
    {
        using var configuredFactory = CreateConfiguredFactory();
        using var client = configuredFactory.CreateClient();
        using var response = await client.PostAsJsonAsync("/api/accounts/register", ValidRequest());
        var rawJson = await response.Content.ReadAsStringAsync();
        var payload = JsonSerializer.Deserialize<AccountRegistrationResponse>(rawJson, JsonOptions);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.DoesNotContain("password", rawJson, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.NotNull(payload.UserId);
        Assert.NotNull(payload.PlayerProfileId);
        Assert.NotNull(payload.CivilizationId);
        Assert.NotNull(payload.HomePlanetId);
        Assert.Equal("Nova Prime", payload.HomePlanetName);
        Assert.Equal($"/planet?civilizationId={payload.CivilizationId}&planetId={payload.HomePlanetId}", payload.NextRoute);
        Assert.Equal(StartingHomeWorldBaseline.StartingCredits, payload.StartingResources?.Credits);
        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var homePlanetId = payload.HomePlanetId!.Value;
        var civilizationId = payload.CivilizationId!.Value;
        Assert.True(await dbContext.Users.AnyAsync(x => x.Email == "player@example.test"));
        Assert.True(await dbContext.PlayerProfiles.AnyAsync(x => x.UserId == payload.UserId));
        Assert.True(await dbContext.Civilizations.AnyAsync(x => x.Id == civilizationId && x.HomePlanetId == homePlanetId));
        Assert.True(await dbContext.Planets.AnyAsync(x => x.Id == homePlanetId));
        Assert.True(await dbContext.PlanetOwnerships.AnyAsync(x => x.PlanetId == homePlanetId && x.CivilizationId == civilizationId && x.Status == PlanetControlStatus.Active));
        Assert.True(await dbContext.PlanetResourceStockpiles.AnyAsync(x => x.PlanetId == homePlanetId && x.Metal == StartingHomeWorldBaseline.StartingMetal));
        Assert.True(await dbContext.PlanetProductionProfiles.AnyAsync(x => x.PlanetId == homePlanetId && x.CreditsPerHour == StartingHomeWorldBaseline.BaseCreditsPerHour));
    }

    [Fact]
    public async Task RegisterReturnsBadRequestForValidationErrors()
    {
        using var configuredFactory = CreateConfiguredFactory();
        using var client = configuredFactory.CreateClient();
        using var response = await client.PostAsJsonAsync("/api/accounts/register", ValidRequest() with
        {
            Email = "not-email",
            Password = "weak",
            ConfirmPassword = "different"
        });
        var payload = await response.Content.ReadFromJsonAsync<AccountRegistrationResponse>();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains(payload.Errors, error => error.Code == "EmailInvalid" && error.Field == "email");
        Assert.Contains(payload.Errors, error => error.Code == "PasswordTooWeak" && error.Field == "password");
        Assert.Contains(payload.Errors, error => error.Code == "PasswordMismatch" && error.Field == "confirmPassword");
    }

    [Fact]
    public async Task RegisterReturnsConflictForDuplicateAccount()
    {
        using var configuredFactory = CreateConfiguredFactory();
        using var client = configuredFactory.CreateClient();
        using var first = await client.PostAsJsonAsync("/api/accounts/register", ValidRequest());
        using var duplicate = await client.PostAsJsonAsync("/api/accounts/register", ValidRequest() with
        {
            DisplayName = "Commander Orion",
            CivilizationName = "Azure League",
            HomePlanetName = "Azure Prime"
        });
        var payload = await duplicate.Content.ReadFromJsonAsync<AccountRegistrationResponse>();
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains(payload.Errors, error => error.Code == "EmailAlreadyRegistered" && error.Field == "email");
    }

    private static AccountRegistrationRequest ValidRequest() =>
        new("player@example.test", "P@ssw0rd!23", "P@ssw0rd!23", "Commander Vega", "Solar Dominion", "Nova Prime");

    private WebApplicationFactory<Program> CreateConfiguredFactory() =>
        factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"))
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddVoidEmpiresIdentity()));

    private sealed record AccountRegistrationResponse(bool Succeeded, string? UserId, Guid? PlayerProfileId, Guid? CivilizationId, Guid? HomePlanetId, string? HomePlanetName, string? NextRoute, CreateStartingCivilizationResourceSnapshot? StartingResources, AccountRegistrationErrorResponse[] Errors);

    private sealed record AccountRegistrationErrorResponse(string Code, string Message, string? Field);
}
