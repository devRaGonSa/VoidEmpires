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
using VoidEmpires.Application.Markets;
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

    [Fact]
    public async Task ApplyCockpitValidationPreservesRealConstructionAndResearchOrdersCreatedThroughDevEndpoints()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();
        Guid? manualConstructionOrderId = null;
        Guid? manualResearchOrderId = null;

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" }))
        {
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        }

        using (var constructionStateResponse = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}"))
        {
            var constructionState = await constructionStateResponse.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();
            Assert.Equal(HttpStatusCode.OK, constructionStateResponse.StatusCode);
            Assert.NotNull(constructionState?.UiState?.Planet);

            var action = constructionState.UiState.Planet.ConstructionActions.First(x => x.AvailabilityStatus == "Available");

            using var enqueueConstructionResponse = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", new
            {
                planetId = SeedOwnedPlanetId,
                civilizationId = SeedCivilizationId,
                action = action.Action,
                buildingType = action.BuildingType,
                requestedAtUtc = "2026-06-04T12:00:00Z"
            });
            var constructionPayload = await enqueueConstructionResponse.Content.ReadFromJsonAsync<EnqueueConstructionOrderResponse>();

            Assert.Equal(HttpStatusCode.Created, enqueueConstructionResponse.StatusCode);
            Assert.NotNull(constructionPayload);
            Assert.True(constructionPayload!.Succeeded);
            Assert.NotNull(constructionPayload.OrderId);
            manualConstructionOrderId = constructionPayload.OrderId;
        }

        using (var researchStateResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}"))
        {
            var researchState = await researchStateResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();
            Assert.Equal(HttpStatusCode.OK, researchStateResponse.StatusCode);
            Assert.NotNull(researchState?.UiState);

            var hint = researchState.UiState.TechnologyHints.First(x => x.CanEnqueue);

            using var enqueueResearchResponse = await client.PostAsJsonAsync(hint.EnqueueCommand!.Route, new
            {
                civilizationId = hint.EnqueueCommand.CivilizationId,
                sourcePlanetId = hint.EnqueueCommand.SourcePlanetId,
                researchType = hint.EnqueueCommand.ResearchType.ToString(),
                requestedAtUtc = "2026-06-04T12:05:00Z"
            });
            var researchPayload = await enqueueResearchResponse.Content.ReadFromJsonAsync<EnqueueResearchOrderResponse>();

            Assert.Equal(HttpStatusCode.Created, enqueueResearchResponse.StatusCode);
            Assert.NotNull(researchPayload);
            Assert.True(researchPayload!.Succeeded);
            Assert.NotNull(researchPayload.OrderId);
            manualResearchOrderId = researchPayload.OrderId;
        }

        using var firstApplyResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        using var secondApplyResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });

        Assert.Equal(HttpStatusCode.OK, firstApplyResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondApplyResponse.StatusCode);
        Assert.NotNull(manualConstructionOrderId);
        Assert.NotNull(manualResearchOrderId);

        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();

        Assert.Equal(2, await dbContext.Set<ResearchOrder>().CountAsync(x => x.CivilizationId == SeedCivilizationId));
        Assert.Equal(2, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x => x.PlanetId == SeedOwnedPlanetId));
        Assert.Equal(1, await dbContext.Set<ResearchOrder>().CountAsync(x =>
            x.CivilizationId == SeedCivilizationId &&
            x.ResearchType == ResearchType.EnergySystems &&
            x.Status == ResearchQueueItemStatus.Completed));
        Assert.Equal(1, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x =>
            x.PlanetId == SeedOwnedPlanetId &&
            x.Status == ConstructionQueueItemStatus.Completed));
        Assert.Equal(1, await dbContext.Set<ResearchOrder>().CountAsync(x =>
            x.CivilizationId == SeedCivilizationId &&
            x.Status == ResearchQueueItemStatus.Active));
        Assert.Equal(1, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x =>
            x.PlanetId == SeedOwnedPlanetId &&
            x.Status == ConstructionQueueItemStatus.Active));
        Assert.True(await dbContext.Set<ResearchOrder>().AnyAsync(x =>
            x.Id == manualResearchOrderId &&
            x.CivilizationId == SeedCivilizationId &&
            x.Status == ResearchQueueItemStatus.Active));
        Assert.True(await dbContext.Set<PlanetConstructionOrder>().AnyAsync(x =>
            x.Id == manualConstructionOrderId &&
            x.PlanetId == SeedOwnedPlanetId &&
            x.Status == ConstructionQueueItemStatus.Active));
        Assert.Equal(1, await dbContext.Set<ResearchOrder>().CountAsync(x =>
            x.CivilizationId == SeedCivilizationId &&
            x.ResearchType == ResearchType.EnergySystems &&
            x.Status == ResearchQueueItemStatus.Completed &&
            x.Sequence >= SeededResearchSequenceStart));
        Assert.Equal(1, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x =>
            x.PlanetId == SeedOwnedPlanetId &&
            x.Status == ConstructionQueueItemStatus.Completed &&
            x.Sequence >= SeededConstructionSequenceStart));
    }

    [Fact]
    public async Task CockpitValidationReadModelsRemainCoherentAfterRealConstructionAndResearchOrdersExist()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" }))
        {
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        }

        using (var constructionStateResponse = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}"))
        {
            var constructionState = await constructionStateResponse.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();
            Assert.Equal(HttpStatusCode.OK, constructionStateResponse.StatusCode);
            Assert.NotNull(constructionState?.UiState?.Planet);

            var action = constructionState.UiState.Planet.ConstructionActions.First(x => x.AvailabilityStatus == "Available");

            using var enqueueConstructionResponse = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", new
            {
                planetId = SeedOwnedPlanetId,
                civilizationId = SeedCivilizationId,
                action = action.Action,
                buildingType = action.BuildingType,
                requestedAtUtc = "2026-06-04T12:10:00Z"
            });

            Assert.Equal(HttpStatusCode.Created, enqueueConstructionResponse.StatusCode);
        }

        using (var researchStateResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}"))
        {
            var researchState = await researchStateResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();
            Assert.Equal(HttpStatusCode.OK, researchStateResponse.StatusCode);
            Assert.NotNull(researchState?.UiState);

            var hint = researchState.UiState.TechnologyHints.First(x => x.CanEnqueue);

            using var enqueueResearchResponse = await client.PostAsJsonAsync(hint.EnqueueCommand!.Route, new
            {
                civilizationId = hint.EnqueueCommand.CivilizationId,
                sourcePlanetId = hint.EnqueueCommand.SourcePlanetId,
                researchType = hint.EnqueueCommand.ResearchType.ToString(),
                requestedAtUtc = "2026-06-04T12:15:00Z"
            });

            Assert.Equal(HttpStatusCode.Created, enqueueResearchResponse.StatusCode);
        }

        using var planetResponse = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        using var marketResponse = await client.GetAsync($"/api/dev/market/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        using var researchResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");

        var planetPayload = await planetResponse.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();
        var marketPayload = await marketResponse.Content.ReadFromJsonAsync<DevMarketUiStateResponse>();
        var researchPayload = await researchResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, planetResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, marketResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, researchResponse.StatusCode);

        Assert.NotNull(planetPayload?.UiState?.Planet);
        Assert.NotNull(marketPayload?.UiState?.Market);
        Assert.NotNull(researchPayload?.UiState);

        Assert.True(planetPayload!.Succeeded);
        Assert.True(marketPayload!.Succeeded);
        Assert.True(researchPayload!.Succeeded);
        Assert.Empty(planetPayload.Errors);
        Assert.Empty(marketPayload.Errors);
        Assert.Empty(researchPayload.Errors);

        Assert.Equal(SeedOwnedPlanetId, planetPayload.UiState.SelectedPlanetId);
        Assert.True(planetPayload.UiState.Planet.IsOwnedByRequestingCivilization);
        Assert.True(planetPayload.UiState.Planet.ConstructionQueue.Length >= 2);
        Assert.Equal("Blocked", planetPayload.UiState.Planet.ActionSummary.QueueActionStatus.ToString());

        Assert.Equal(SeedOwnedPlanetId, marketPayload.UiState.SelectedPlanetId);
        Assert.Equal("Aurelia", marketPayload.UiState.Market.SelectedPlanetName);
        Assert.NotEmpty(marketPayload.UiState.Market.SelectedPlanetReserves);
        Assert.NotNull(marketPayload.UiState.Market.SelectedPlanetProduction);
        Assert.Contains(marketPayload.UiState.Market.Signals, x => x.SignalKey == "FutureTradeRoute");
        Assert.All(marketPayload.UiState.Market.FutureActions, x => Assert.False(x.IsEnabled));

        Assert.Equal(SeedOwnedPlanetId, researchPayload.UiState.SelectedPlanetId);
        Assert.Equal("Aurelia", researchPayload.UiState.SelectedPlanetName);
        Assert.Contains(researchPayload.UiState.Queue, x => x.Status == ResearchQueueItemStatus.Active);
        Assert.Contains(researchPayload.UiState.Projects, x => x.ResearchType == ResearchType.EnergySystems);
        Assert.Contains(researchPayload.UiState.TechnologyHints, x => x.ResearchType == ResearchType.PlanetaryEngineering && !x.CanEnqueue && x.StatusKey == "InResearch");
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

    private sealed record DevPlanetUiStateResponse(
        bool Succeeded,
        DevPlanetUiStateResult? UiState,
        string[] Errors);

    private sealed record DevPlanetUiStateResult(
        Guid CivilizationId,
        Guid? SelectedPlanetId,
        DevPlanetCockpitDto? Planet,
        string[] Errors);

    private sealed record DevPlanetCockpitDto(
        Guid PlanetId,
        string PlanetName,
        Guid SolarSystemId,
        string SolarSystemName,
        int OrbitalSlot,
        object PlanetType,
        int Size,
        object ColonizationStatus,
        bool IsOwnedByRequestingCivilization,
        Guid? OwnerCivilizationId,
        string? OwnerCivilizationName,
        object? ControlStatus,
        object[] Stockpile,
        object? ProductionSummary,
        object? BuildingCapacity,
        object[] Buildings,
        object[] ConstructionQueue,
        DevPlanetConstructionActionSummaryDto ActionSummary,
        DevPlanetConstructionActionDto[] ConstructionActions,
        object OrbitalContext,
        object Diagnostics);

    private sealed record DevPlanetConstructionActionSummaryDto(
        string QueueActionStatus,
        string QueueActionReason,
        bool CompleteDueSupported,
        string CompleteDueActionStatus,
        string CompleteDueActionReason,
        int DueConstructionCount,
        object? Display);

    private sealed record DevPlanetConstructionActionDto(
        ConstructionQueueItemAction Action,
        BuildingType BuildingType,
        object Category,
        int CurrentLevel,
        int TargetLevel,
        string AvailabilityStatus,
        string AvailabilityReason,
        TimeSpan EstimatedDuration,
        object[] Cost,
        object? Display);

    private sealed record EnqueueConstructionOrderResponse(
        bool Succeeded,
        Guid? OrderId,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
        IReadOnlyList<string> Errors);

    private sealed record EnqueueResearchOrderResponse(
        bool Succeeded,
        Guid? OrderId,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
        IReadOnlyList<string> Errors);

    private sealed record DevResearchUiStateResponse(
        bool Succeeded,
        DevResearchUiStateResult? UiState,
        string[] Errors);

    private sealed record DevResearchUiStateResult(
        Guid CivilizationId,
        Guid? SelectedPlanetId,
        string? SelectedPlanetName,
        object[] Catalog,
        DevResearchOrderDto[] Queue,
        DevResearchProjectDto[] Projects,
        DevResearchTechnologyHintDto[] TechnologyHints,
        string[] Diagnostics,
        string[] Limitations);

    private sealed record DevResearchOrderDto(
        Guid Id,
        Guid CivilizationId,
        Guid SourcePlanetId,
        ResearchType ResearchType,
        int TargetLevel,
        int Sequence,
        DateTime StartsAtUtc,
        DateTime EndsAtUtc,
        ResearchQueueItemStatus Status);

    private sealed record DevResearchProjectDto(
        Guid CivilizationId,
        ResearchType ResearchType,
        int Level);

    private sealed record DevResearchTechnologyHintDto(
        ResearchType ResearchType,
        int CurrentLevel,
        int NextLevel,
        string StatusKey,
        string AvailabilityReasonKey,
        bool CanEnqueue,
        bool CanCompleteDue,
        TimeSpan EstimatedDuration,
        ResearchCost EstimatedCost,
        DevResearchEnqueueCommandDto? EnqueueCommand,
        IReadOnlyList<string> RequirementKeys);

    private sealed record DevResearchEnqueueCommandDto(
        string ActionKey,
        string Method,
        string Route,
        Guid CivilizationId,
        Guid SourcePlanetId,
        ResearchType ResearchType,
        int TargetLevel);

    private sealed record DevelopmentSeedProfilesResponse(
        bool Succeeded,
        DevelopmentSeedProfileSummary[] Profiles,
        string[] Errors);

    private sealed record DevMarketUiStateResponse(
        bool Succeeded,
        GetDevMarketUiStateResult? UiState,
        string[] Errors);
}
