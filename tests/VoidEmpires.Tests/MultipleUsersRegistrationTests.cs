using System.Net;
using System.Net.Http.Json;
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

public class MultipleUsersRegistrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task RegisteringTwoUsersCreatesDistinctOwnedHomeWorldsWithEqualBaseline()
    {
        using var configuredFactory = CreateConfiguredFactory();
        using var client = configuredFactory.CreateClient();

        var first = await RegisterAsync(client, ValidRequest());
        var second = await RegisterAsync(client, ValidRequest(
            "orion@example.test",
            "Commander Orion",
            "Azure League",
            "Azure Prime"));

        Assert.NotEqual(first.UserId, second.UserId);
        Assert.NotEqual(first.PlayerProfileId, second.PlayerProfileId);
        Assert.NotEqual(first.CivilizationId, second.CivilizationId);
        Assert.NotEqual(first.HomePlanetId, second.HomePlanetId);
        Assert.Equal(first.StartingResources, second.StartingResources);

        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var ownerships = await dbContext.PlanetOwnerships
            .Where(x => x.Status == PlanetControlStatus.Active)
            .Select(x => new { x.PlanetId, x.CivilizationId })
            .ToListAsync();

        Assert.Equal(2, await dbContext.PlayerProfiles.CountAsync());
        Assert.Equal(2, await dbContext.Civilizations.CountAsync());
        Assert.Equal(2, ownerships.Select(x => x.PlanetId).Distinct().Count());
        Assert.Equal(2, ownerships.Select(x => x.CivilizationId).Distinct().Count());
        await AssertBaselineAsync(dbContext, first.HomePlanetId!.Value);
        await AssertBaselineAsync(dbContext, second.HomePlanetId!.Value);
    }

    private static async Task<AccountRegistrationResponse> RegisterAsync(HttpClient client, AccountRegistrationRequest request)
    {
        using var response = await client.PostAsJsonAsync("/api/accounts/register", request);
        var payload = await response.Content.ReadFromJsonAsync<AccountRegistrationResponse>();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.NotNull(payload.UserId);
        Assert.NotNull(payload.PlayerProfileId);
        Assert.NotNull(payload.CivilizationId);
        Assert.NotNull(payload.HomePlanetId);
        return payload;
    }

    private static async Task AssertBaselineAsync(VoidEmpiresDbContext dbContext, Guid planetId)
    {
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == planetId);
        var production = await dbContext.PlanetProductionProfiles.SingleAsync(x => x.PlanetId == planetId);

        Assert.Equal(StartingHomeWorldBaseline.StartingCredits, stockpile.Credits);
        Assert.Equal(StartingHomeWorldBaseline.StartingMetal, stockpile.Metal);
        Assert.Equal(StartingHomeWorldBaseline.StartingCrystal, stockpile.Crystal);
        Assert.Equal(StartingHomeWorldBaseline.StartingGas, stockpile.Gas);
        Assert.Equal(StartingHomeWorldBaseline.BaseCreditsPerHour, production.CreditsPerHour);
        Assert.Equal(StartingHomeWorldBaseline.BaseMetalPerHour, production.MetalPerHour);
        Assert.Equal(StartingHomeWorldBaseline.BaseCrystalPerHour, production.CrystalPerHour);
        Assert.Equal(StartingHomeWorldBaseline.BaseGasPerHour, production.GasPerHour);
    }

    private static AccountRegistrationRequest ValidRequest(
        string email = "player@example.test",
        string displayName = "Commander Vega",
        string civilizationName = "Solar Dominion",
        string homePlanetName = "Nova Prime") =>
        new(email, "P@ssw0rd!23", "P@ssw0rd!23", displayName, civilizationName, homePlanetName);

    private WebApplicationFactory<Program> CreateConfiguredFactory() =>
        factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"))
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddVoidEmpiresIdentity()));

    private sealed record AccountRegistrationResponse(
        bool Succeeded,
        string? UserId,
        Guid? PlayerProfileId,
        Guid? CivilizationId,
        Guid? HomePlanetId,
        string? HomePlanetName,
        string? NextRoute,
        CreateStartingCivilizationResourceSnapshot? StartingResources,
        AccountRegistrationErrorResponse[] Errors);

    private sealed record AccountRegistrationErrorResponse(string Code, string Message, string? Field);
}
