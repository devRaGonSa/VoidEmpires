using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevPlayableSessionDiagnosticsEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOuterPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");

    [Fact]
    public async Task DiagnosticsReturnsSeededPlayableSummary()
    {
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"));
        using var client = configuredFactory.CreateClient();

        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        using var response = await client.GetAsync($"/api/dev/playable-session/diagnostics?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DiagnosticsEnvelope>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.Diagnostics);
        Assert.True(payload.Succeeded);
        Assert.Equal(SeedCivilizationId, payload.Diagnostics.CivilizationId);
        Assert.Equal(SeedOwnedPlanetId, payload.Diagnostics.PlanetId);
        Assert.Equal("Aurelia", payload.Diagnostics.PlanetName);
        Assert.Contains(payload.Diagnostics.Resources, x => x.ResourceType == "Credits");
        Assert.True(payload.Diagnostics.Construction.OpenCount >= 0);
        Assert.True(payload.Diagnostics.Research.OpenCount >= 0);
        Assert.True(payload.Diagnostics.Shipyard.OpenCount >= 0);
        Assert.Contains(payload.Diagnostics.Limitations, x => x.Contains("Read-only", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task DiagnosticsReadsPlayableStartHomeworldWithoutMutatingState()
    {
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"));
        using var client = configuredFactory.CreateClient();

        using var startResponse = await client.PostAsJsonAsync("/api/dev/players/starting-civilization", new
        {
            displayName = "Flow Tester",
            civilizationName = "Smoke Dominion",
            homePlanetName = "Testhaven",
            userId = "dev-smoke-flow"
        });
        var startPayload = await startResponse.Content.ReadFromJsonAsync<StartingCivilizationPayload>();

        Assert.Equal(HttpStatusCode.Created, startResponse.StatusCode);
        Assert.NotNull(startPayload);
        Assert.True(startPayload!.Succeeded);
        Assert.NotNull(startPayload.CivilizationId);
        Assert.NotNull(startPayload.HomePlanetId);

        var civilizationId = startPayload.CivilizationId.Value;
        var homePlanetId = startPayload.HomePlanetId.Value;
        var before = await CaptureSnapshotAsync(configuredFactory.Services, civilizationId, homePlanetId);

        using var diagnosticsResponse = await client.GetAsync($"/api/dev/playable-session/diagnostics?civilizationId={civilizationId}&planetId={homePlanetId}");
        var diagnosticsPayload = await diagnosticsResponse.Content.ReadFromJsonAsync<DiagnosticsEnvelope>();

        Assert.Equal(HttpStatusCode.OK, diagnosticsResponse.StatusCode);
        Assert.NotNull(diagnosticsPayload?.Diagnostics);
        Assert.True(diagnosticsPayload.Succeeded);
        Assert.Equal(civilizationId, diagnosticsPayload.Diagnostics.CivilizationId);
        Assert.Equal(homePlanetId, diagnosticsPayload.Diagnostics.PlanetId);
        Assert.Equal("Testhaven", diagnosticsPayload.Diagnostics.PlanetName);
        Assert.Contains(diagnosticsPayload.Diagnostics.Resources, x => x.ResourceType == "Credits" && x.Quantity == 220);
        Assert.Contains(diagnosticsPayload.Diagnostics.Resources, x => x.ResourceType == "Metal" && x.Quantity == 320);
        Assert.Equal(0, diagnosticsPayload.Diagnostics.Construction.OpenCount);
        Assert.Equal(0, diagnosticsPayload.Diagnostics.Research.OpenCount);
        Assert.Equal(0, diagnosticsPayload.Diagnostics.Shipyard.OpenCount);
        Assert.Contains(diagnosticsPayload.Diagnostics.Limitations, x => x.Contains("Read-only", StringComparison.OrdinalIgnoreCase));

        var after = await CaptureSnapshotAsync(configuredFactory.Services, civilizationId, homePlanetId);
        Assert.Equal(before, after);
    }

    [Fact]
    public async Task DiagnosticsReturnsBadRequestForInvalidIds()
    {
        using var client = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/playable-session/diagnostics?civilizationId={Guid.Empty}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DiagnosticsEnvelope>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Null(payload.Diagnostics);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task DiagnosticsDoesNotMutatePlayableState()
    {
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"));
        using var client = configuredFactory.CreateClient();

        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        var before = await CaptureSnapshotAsync(configuredFactory.Services, SeedCivilizationId, SeedOwnedPlanetId);

        using var response = await client.GetAsync($"/api/dev/playable-session/diagnostics?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var after = await CaptureSnapshotAsync(configuredFactory.Services, SeedCivilizationId, SeedOwnedPlanetId);
        Assert.Equal(before, after);
    }

    [Fact]
    public async Task DiagnosticsDoesNotLeakUnownedPlanetState()
    {
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"));
        using var client = configuredFactory.CreateClient();

        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        using var response = await client.GetAsync($"/api/dev/playable-session/diagnostics?civilizationId={SeedCivilizationId}&planetId={SeedOuterPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DiagnosticsEnvelope>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Null(payload.Diagnostics);
        Assert.Contains("Planet was not found for the requested civilization.", payload.Errors);
    }

    [Fact]
    public async Task DiagnosticsReturnsServiceUnavailableForPersistenceReadFailure()
    {
        var brokenFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_diagnostics_unavailable"
                });
            });

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<VoidEmpiresDbContext>>();
                services.RemoveAll<VoidEmpiresDbContext>();
                services.AddScoped(_ =>
                {
                    var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                        .Options;
                    var dbContext = new VoidEmpiresDbContext(options);
                    dbContext.Dispose();
                    return dbContext;
                });
            });
        });

        using var client = brokenFactory.CreateClient();
        using var response = await client.GetAsync($"/api/dev/playable-session/diagnostics?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DiagnosticsEnvelope>();

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Null(payload.Diagnostics);
        Assert.Contains("Diagnostics are temporarily unavailable because the configured database could not be reached.", payload.Errors);
        Assert.Contains("This read-only diagnostics request did not apply any data changes.", payload.Errors);
    }

    private static async Task<DiagnosticsMutationSnapshot> CaptureSnapshotAsync(IServiceProvider services, Guid civilizationId, Guid planetId)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var stockpile = await dbContext.Set<PlanetResourceStockpile>()
            .AsNoTracking()
            .SingleAsync(x => x.PlanetId == planetId);

        return new DiagnosticsMutationSnapshot(
            stockpile.Credits,
            stockpile.Metal,
            stockpile.Crystal,
            stockpile.Gas,
            await dbContext.Set<PlanetConstructionOrder>().CountAsync(x => x.PlanetId == planetId),
            await dbContext.Set<PlanetConstructionOrder>().CountAsync(x => x.PlanetId == planetId && (x.Status == ConstructionQueueItemStatus.Pending || x.Status == ConstructionQueueItemStatus.Active)),
            await dbContext.Set<PlanetConstructionOrder>().CountAsync(x => x.PlanetId == planetId && x.Status == ConstructionQueueItemStatus.Completed),
            await dbContext.Set<ResearchOrder>().CountAsync(x => x.CivilizationId == civilizationId),
            await dbContext.Set<ResearchOrder>().CountAsync(x => x.CivilizationId == civilizationId && (x.Status == ResearchQueueItemStatus.Pending || x.Status == ResearchQueueItemStatus.Active)),
            await dbContext.Set<ResearchOrder>().CountAsync(x => x.CivilizationId == civilizationId && x.Status == ResearchQueueItemStatus.Completed),
            await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == planetId),
            await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == planetId && (x.Status == AssetProductionOrderStatus.Pending || x.Status == AssetProductionOrderStatus.Active)),
            await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == planetId && x.Status == AssetProductionOrderStatus.Completed),
            await dbContext.Set<PlanetBuilding>().CountAsync(x => x.PlanetId == planetId),
            await dbContext.Set<ResearchProject>().CountAsync(x => x.CivilizationId == civilizationId),
            await dbContext.Set<OrbitalAssetStock>().SumAsync(x => x.PlanetId == planetId ? x.Quantity : 0));
    }

    private sealed record StartingCivilizationPayload(
        bool Succeeded,
        Guid? CivilizationId,
        Guid? HomePlanetId,
        string[] Errors);

    private sealed record DiagnosticsEnvelope(
        bool Succeeded,
        DiagnosticsPayload? Diagnostics,
        string[] Errors);

    private sealed record DiagnosticsPayload(
        Guid CivilizationId,
        Guid PlanetId,
        string? PlanetName,
        ResourceDiagnostic[] Resources,
        QueueDiagnostic Construction,
        QueueDiagnostic Research,
        QueueDiagnostic Shipyard,
        StockDiagnostic[] OrbitalStock,
        string[] ReadinessNotes,
        string[] Warnings,
        string[] Limitations);

    private sealed record ResourceDiagnostic(string ResourceType, decimal Quantity);

    private sealed record QueueDiagnostic(
        int OpenCount,
        Guid? NextOpenOrderId,
        string? Status,
        string? ItemType,
        int? TargetLevel,
        int? Quantity,
        DateTime? EndsAtUtc);

    private sealed record StockDiagnostic(string AssetType, int Quantity);

    private sealed record DiagnosticsMutationSnapshot(
        decimal Credits,
        decimal Metal,
        decimal Crystal,
        decimal Gas,
        int ConstructionOrderCount,
        int OpenConstructionOrderCount,
        int CompletedConstructionOrderCount,
        int ResearchOrderCount,
        int OpenResearchOrderCount,
        int CompletedResearchOrderCount,
        int AssetProductionOrderCount,
        int OpenAssetProductionOrderCount,
        int CompletedAssetProductionOrderCount,
        int BuildingCount,
        int ResearchProjectCount,
        int OrbitalStockQuantity);
}
