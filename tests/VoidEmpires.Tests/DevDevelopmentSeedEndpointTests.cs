using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Research;
using VoidEmpires.Application.Development;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevDevelopmentSeedEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private const int SeededConstructionSequenceStart = 10_000;
    private const int SeededResearchSequenceStart = 20_000;
    private const int SeededAssetProductionSequenceStart = 30_000;

    [Fact]
    public async Task GetSeedProfilesReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync("/api/dev/seeds/profiles");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSeedProfilesReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.GetAsync("/api/dev/seeds/profiles");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task GetSeedProfilesReturnsKnownProfilesForDevelopmentDiscovery()
    {
        using var client = CreateConfiguredClient(ApplyDevelopmentSeedResult.Success(
            "minimal-validation",
            [],
            DevelopmentSeedProfiles.MinimalValidation,
            DevelopmentSeedProfiles.All));

        using var response = await client.GetAsync("/api/dev/seeds/profiles");
        var payload = await response.Content.ReadFromJsonAsync<DevelopmentSeedProfilesResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Contains(payload.Profiles, x => x.Name == "minimal-validation" && !x.Destructive && x.Deterministic);
        Assert.Contains(payload.Profiles, x => x.Name == "cockpit-validation" && x.IntendedCockpits.Contains("Research"));
        Assert.Contains(payload.Profiles, x => x.Name == "research-validation" && x.RecommendedQaUrls.Contains("/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001"));
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task ApplySeedReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "minimal-validation" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ApplySeedReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "minimal-validation" });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task ApplySeedReturnsBadRequestForMissingProfile()
    {
        using var client = CreateConfiguredClient(ApplyDevelopmentSeedResult.Success(
            "minimal-validation",
            [],
            DevelopmentSeedProfiles.MinimalValidation,
            DevelopmentSeedProfiles.All));

        using var response = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "" });
        var payload = await response.Content.ReadFromJsonAsync<ApplyDevelopmentSeedResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Seed profile is required.", payload.Errors);
    }

    [Fact]
    public async Task ApplySeedReturnsOkForSuccessfulProfile()
    {
        using var client = CreateConfiguredClient(ApplyDevelopmentSeedResult.Success(
            "minimal-validation",
            ["Seed profile acknowledged."],
            DevelopmentSeedProfiles.MinimalValidation,
            DevelopmentSeedProfiles.All));

        using var response = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "minimal-validation" });
        var payload = await response.Content.ReadFromJsonAsync<ApplyDevelopmentSeedResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal("minimal-validation", payload.Profile);
        Assert.Contains("Seed profile acknowledged.", payload.AppliedSteps);
        Assert.NotNull(payload.ProfileMetadata);
        Assert.Equal("minimal-validation", payload.ProfileMetadata.Name);
        Assert.Contains(payload.KnownProfiles, x => x.Name == "research-validation" && x.IsImplemented);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task ApplySeedReturnsConflictForPersistedStateFailure()
    {
        using var client = CreateConfiguredClient(new ThrowingDevelopmentSeedService(
            new DbUpdateException("Persisted state conflict.", new InvalidOperationException("duplicate key value violates unique constraint"))));

        using var response = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        var payload = await response.Content.ReadFromJsonAsync<ApplyDevelopmentSeedResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Development seed apply failed due to persisted state conflict.", payload.Errors);
        Assert.Contains(payload.Errors, x => x.Contains("duplicate key value violates unique constraint", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(payload.KnownProfiles, x => x.Name == "cockpit-validation" && x.IsImplemented);
    }

    [Fact]
    public async Task ApplyCockpitValidationRemainsStableAfterManualQueueStateOnSharedDevelopmentDatabase()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "minimal-validation" }))
        {
            var seedPayload = await seedResponse.Content.ReadFromJsonAsync<ApplyDevelopmentSeedResponse>();
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
            Assert.NotNull(seedPayload);
            Assert.True(seedPayload!.Succeeded);
        }

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
            dbContext.Set<ResearchOrder>().Add(ResearchOrder.Create(
                SeedCivilizationId,
                SeedOwnedPlanetId,
                ResearchType.PlanetaryEngineering,
                1,
                1,
                new DateTime(2026, 1, 3, 10, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 1, 3, 10, 15, 0, DateTimeKind.Utc),
                ResearchQueueItemStatus.Active));
            dbContext.Set<PlanetConstructionOrder>().Add(PlanetConstructionOrder.Create(
                SeedOwnedPlanetId,
                ConstructionQueueItemAction.Upgrade,
                BuildingType.CommandCenter,
                5,
                1,
                new DateTime(2026, 1, 3, 11, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 1, 3, 11, 10, 0, DateTimeKind.Utc),
                ConstructionQueueItemStatus.Active));
            dbContext.Set<AssetProductionOrder>().Add(AssetProductionOrder.Create(
                SeedOwnedPlanetId,
                AssetProductionTarget.Orbital,
                null,
                SpaceAssetType.CargoCraft,
                1,
                1,
                new DateTime(2026, 1, 3, 12, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 1, 3, 12, 5, 0, DateTimeKind.Utc),
                AssetProductionOrderStatus.Active));
            await dbContext.SaveChangesAsync();
        }

        using var firstApplyResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        var firstApplyPayload = await firstApplyResponse.Content.ReadFromJsonAsync<ApplyDevelopmentSeedResponse>();
        using var secondApplyResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        var secondApplyPayload = await secondApplyResponse.Content.ReadFromJsonAsync<ApplyDevelopmentSeedResponse>();

        Assert.Equal(HttpStatusCode.OK, firstApplyResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondApplyResponse.StatusCode);
        Assert.NotNull(firstApplyPayload);
        Assert.NotNull(secondApplyPayload);
        Assert.True(firstApplyPayload!.Succeeded);
        Assert.True(secondApplyPayload!.Succeeded);

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
            Assert.Equal(2, await dbContext.Set<ResearchOrder>().CountAsync(x => x.CivilizationId == SeedCivilizationId));
            Assert.Equal(2, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x => x.PlanetId == SeedOwnedPlanetId));
            Assert.Equal(3, await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == SeedOwnedPlanetId));
            Assert.Equal(1, await dbContext.Set<ResearchOrder>().CountAsync(x => x.CivilizationId == SeedCivilizationId && x.Sequence == 1));
            Assert.Equal(1, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x => x.PlanetId == SeedOwnedPlanetId && x.Sequence == 1));
            Assert.Equal(1, await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == SeedOwnedPlanetId && x.Sequence == 1));
            Assert.Equal(1, await dbContext.Set<ResearchOrder>().CountAsync(x => x.CivilizationId == SeedCivilizationId && x.Sequence >= SeededResearchSequenceStart));
            Assert.Equal(1, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x => x.PlanetId == SeedOwnedPlanetId && x.Sequence >= SeededConstructionSequenceStart));
            Assert.Equal(2, await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == SeedOwnedPlanetId && x.Sequence >= SeededAssetProductionSequenceStart));
        }

        using var researchResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        using var shipyardResponse = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        using var planetResponse = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        using var fleetResponse = await client.GetAsync($"/api/dev/fleets/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.OK, researchResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, shipyardResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, planetResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, fleetResponse.StatusCode);
    }

    private HttpClient CreateConfiguredClient(ApplyDevelopmentSeedResult result) =>
        CreateConfiguredClient(new FakeDevelopmentSeedService(result));

    private HttpClient CreateConfiguredClient(IDevelopmentSeedService service) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_seed_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
                services.AddSingleton(service));
        }).CreateClient();

    private sealed class FakeDevelopmentSeedService(ApplyDevelopmentSeedResult result) : IDevelopmentSeedService
    {
        public Task<ApplyDevelopmentSeedResult> ApplyAsync(
            ApplyDevelopmentSeedRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed class ThrowingDevelopmentSeedService(Exception exception) : IDevelopmentSeedService
    {
        public Task<ApplyDevelopmentSeedResult> ApplyAsync(
            ApplyDevelopmentSeedRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromException<ApplyDevelopmentSeedResult>(exception);
    }

    private sealed record ApplyDevelopmentSeedResponse(
        bool Succeeded,
        string? Profile,
        string[] AppliedSteps,
        string[] Errors,
        DevelopmentSeedProfileMetadata? ProfileMetadata,
        DevelopmentSeedProfileMetadata[] KnownProfiles);

    private sealed record DevelopmentSeedProfilesResponse(
        bool Succeeded,
        DevelopmentSeedProfileSummary[] Profiles,
        string[] Errors);
}
