using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Markets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevMarketUiStateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task MarketUiStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/market/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task MarketUiStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.GetAsync($"/api/dev/market/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task MarketUiStateReturnsBadRequestWhenCivilizationIdIsMissing()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync("/api/dev/market/ui-state");
        var payload = await response.Content.ReadFromJsonAsync<DevMarketUiStateResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task MarketUiStateReturnsNotFoundForUnknownCivilization()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync($"/api/dev/market/ui-state?civilizationId={Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<DevMarketUiStateResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization was not found.", payload.Errors);
    }

    [Fact]
    public async Task MarketUiStateReturnsNotFoundForUnknownPlanet()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync($"/api/dev/market/ui-state?civilizationId={SeedCivilizationId}&planetId={Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<DevMarketUiStateResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Planet was not found.", payload.Errors);
    }

    [Fact]
    public async Task MarketUiStateReturnsCockpitValidationReadModelWithoutMutatingState()
    {
        await using var dbContext = CreateSeededDbContext(profile: "cockpit-validation");
        var stockpileBefore = await dbContext.PlanetResourceStockpiles
            .AsNoTracking()
            .Where(x => x.PlanetId == SeedOwnedPlanetId)
            .Select(x => new { x.Credits, x.Metal, x.Crystal, x.Gas })
            .SingleAsync();
        var transferCountBefore = await dbContext.Set<VoidEmpires.Domain.Fleets.OrbitalTransfer>().CountAsync();

        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/market/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevMarketUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Market);
        Assert.True(payload.Succeeded);
        Assert.Equal(SeedOwnedPlanetId, payload.UiState.SelectedPlanetId);
        Assert.Equal("Aurelia", payload.UiState.Market.SelectedPlanetName);
        Assert.NotEmpty(payload.UiState.KnownPlanets);
        Assert.NotEmpty(payload.UiState.Market.CivilizationReserves);
        Assert.NotEmpty(payload.UiState.Market.SelectedPlanetReserves);
        Assert.NotNull(payload.UiState.Market.SelectedPlanetProduction);
        Assert.Equal(4, payload.UiState.Market.ReferenceRatios.Count);
        Assert.Contains(payload.UiState.Market.Signals, x => x.SignalKey == "FutureTradeRoute");
        Assert.Equal(5, payload.UiState.Market.FutureActions.Count);
        Assert.Contains(payload.UiState.Market.CivilizationReserves, x => x.ResourceType == ResourceType.Credits && x.Quantity >= 220);
        Assert.Contains(payload.UiState.Market.SelectedPlanetReserves, x => x.ResourceType == ResourceType.Metal && x.Quantity >= 320);
        Assert.Contains(payload.UiState.Market.ReferenceRatios, x => x.ResourceType == ResourceType.Gas && x.IsAdvisory);
        Assert.Contains(payload.UiState.Market.Limitations, x => x.Contains("read-only", StringComparison.OrdinalIgnoreCase));

        var stockpileAfter = await dbContext.PlanetResourceStockpiles
            .AsNoTracking()
            .Where(x => x.PlanetId == SeedOwnedPlanetId)
            .Select(x => new { x.Credits, x.Metal, x.Crystal, x.Gas })
            .SingleAsync();
        var transferCountAfter = await dbContext.Set<VoidEmpires.Domain.Fleets.OrbitalTransfer>().CountAsync();

        Assert.Equal(stockpileBefore.Credits, stockpileAfter.Credits);
        Assert.Equal(stockpileBefore.Metal, stockpileAfter.Metal);
        Assert.Equal(stockpileBefore.Crystal, stockpileAfter.Crystal);
        Assert.Equal(stockpileBefore.Gas, stockpileAfter.Gas);
        Assert.Equal(transferCountBefore, transferCountAfter);
    }

    private HttpClient CreateConfiguredClient(VoidEmpiresDbContext dbContext) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_market_ui_state_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(dbContext);
            });
        }).CreateClient();

    private static VoidEmpiresDbContext CreateSeededDbContext(string profile = "minimal-validation")
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest(profile)).GetAwaiter().GetResult();
        return dbContext;
    }

    private sealed record DevMarketUiStateResponse(
        bool Succeeded,
        GetDevMarketUiStateResult? UiState,
        string[] Errors);
}
