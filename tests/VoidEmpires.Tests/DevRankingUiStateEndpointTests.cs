using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Rankings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevRankingUiStateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task RankingUiStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/ranking/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RankingUiStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.GetAsync($"/api/dev/ranking/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task RankingUiStateReturnsBadRequestWhenCivilizationIdIsMissing()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync("/api/dev/ranking/ui-state");
        var payload = await response.Content.ReadFromJsonAsync<DevRankingUiStateResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task RankingUiStateReturnsNotFoundForUnknownCivilization()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync($"/api/dev/ranking/ui-state?civilizationId={Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<DevRankingUiStateResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization was not found.", payload.Errors);
    }

    [Fact]
    public async Task RankingUiStateReturnsDeterministicReadModelWithoutMutatingState()
    {
        await using var dbContext = CreateSeededDbContext("cockpit-validation");
        var stockpileBefore = await dbContext.Set<PlanetResourceStockpile>()
            .AsNoTracking()
            .Where(x => x.PlanetId == SeedOwnedPlanetId)
            .Select(x => new { x.Credits, x.Metal, x.Crystal, x.Gas })
            .SingleAsync();
        var transferCountBefore = await dbContext.Set<OrbitalTransfer>().CountAsync();

        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/ranking/ui-state?civilizationId={SeedCivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<DevRankingUiStateResponse>();

        var stockpileAfter = await dbContext.Set<PlanetResourceStockpile>()
            .AsNoTracking()
            .Where(x => x.PlanetId == SeedOwnedPlanetId)
            .Select(x => new { x.Credits, x.Metal, x.Crystal, x.Gas })
            .SingleAsync();
        var transferCountAfter = await dbContext.Set<OrbitalTransfer>().CountAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState);
        Assert.NotNull(payload.UiState.Identity);
        Assert.NotNull(payload.UiState.Summary);
        Assert.NotNull(payload.UiState.Publication);
        Assert.NotNull(payload.UiState.Diagnostics);
        Assert.True(payload.Succeeded);
        Assert.Equal(SeedCivilizationId, payload.UiState.Identity.CivilizationId);
        Assert.Equal("Void Seed Civilization", payload.UiState.Identity.CivilizationName);
        Assert.True(payload.UiState.Summary.TotalPowerIndex > 0);
        Assert.NotEmpty(payload.UiState.Summary.Categories);
        Assert.Contains(payload.UiState.Summary.Categories, x => x.CategoryKey == "economicPower" && x.Score > 0);
        Assert.Equal(3, payload.UiState.DemoComparisons.Count);
        Assert.All(payload.UiState.DemoComparisons, x => Assert.True(x.IsDemoOnly));
        Assert.False(payload.UiState.Publication.IsPublished);
        Assert.Equal(3, payload.UiState.FuturePlaceholders.Count);
        Assert.Equal(3, payload.UiState.DisabledActions.Count);
        Assert.Contains(payload.UiState.Limitations, x => x.Contains("read-only", StringComparison.OrdinalIgnoreCase));

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
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_ranking_ui_state_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(dbContext));
        }).CreateClient();

    private static VoidEmpiresDbContext CreateSeededDbContext(string profile = "minimal-validation")
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest(profile)).GetAwaiter().GetResult();
        return dbContext;
    }

    private sealed record DevRankingUiStateResponse(
        bool Succeeded,
        GetDevRankingUiStateResult? UiState,
        string[] Errors);
}
