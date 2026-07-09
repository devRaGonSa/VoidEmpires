using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevShipyardEnqueueEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOuterPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");

    [Fact]
    public async Task PrepareOrbitalQaStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/shipyard/qa-state/prepare", new { });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PrepareOrbitalQaStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/shipyard/qa-state/prepare", new { });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task PrepareOrbitalQaStateCancelsOpenOrdersAndRestoresEnqueueReadiness()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" }))
        {
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        }

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
            var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == SeedOwnedPlanetId);
            dbContext.Entry(stockpile).Property(x => x.Credits).CurrentValue = 0m;
            dbContext.Entry(stockpile).Property(x => x.Metal).CurrentValue = 0m;
            dbContext.Entry(stockpile).Property(x => x.Crystal).CurrentValue = 0m;
            dbContext.Entry(stockpile).Property(x => x.Gas).CurrentValue = 0m;

            var sequence = await dbContext.Set<AssetProductionOrder>()
                .Where(x => x.PlanetId == SeedOwnedPlanetId)
                .Select(x => (int?)x.Sequence)
                .MaxAsync() ?? 0;

            dbContext.Set<AssetProductionOrder>().Add(AssetProductionOrder.Create(
                SeedOwnedPlanetId,
                AssetProductionTarget.Orbital,
                null,
                SpaceAssetType.CargoCraft,
                1,
                sequence + 1,
                new DateTime(2026, 6, 8, 12, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 6, 8, 12, 10, 0, DateTimeKind.Utc),
                AssetProductionOrderStatus.Active));

            await dbContext.SaveChangesAsync();
        }

        using var response = await client.PostAsJsonAsync("/api/dev/shipyard/qa-state/prepare", new { });
        var payload = await response.Content.ReadFromJsonAsync<PrepareOrbitalQaStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
        Assert.Equal(1, payload.BlockingOrdersBefore);
        Assert.Equal(0, payload.BlockingOrdersAfter);
        Assert.NotNull(payload.ResourcesAfter);
        Assert.True(payload.ResourcesAfter!.Credits >= 125m);
        Assert.True(payload.ResourcesAfter.Metal >= 160m);
        Assert.True(payload.ResourcesAfter.Crystal >= 100m);
        Assert.True(payload.ResourcesAfter.Gas >= 50m);

        using var uiStateResponse = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var uiStatePayload = await uiStateResponse.Content.ReadFromJsonAsync<ShipyardUiStateEnvelope>();

        Assert.Equal(HttpStatusCode.OK, uiStateResponse.StatusCode);
        Assert.NotNull(uiStatePayload?.UiState?.Shipyard);
        Assert.True(uiStatePayload.UiState.Shipyard.ActionSummary.EnqueueSupported);

        var availableItem = Assert.Single(uiStatePayload.UiState.Shipyard.Catalog.Where(item => item.AssetType == SpaceAssetType.ScoutCraft));
        Assert.NotNull(availableItem.EnqueueCommand);

        using var enqueueResponse = await client.PostAsJsonAsync(availableItem.EnqueueCommand.Route, new
        {
            civilizationId = availableItem.EnqueueCommand.CivilizationId,
            planetId = availableItem.EnqueueCommand.PlanetId,
            target = availableItem.EnqueueCommand.Target,
            spaceAssetType = availableItem.EnqueueCommand.SpaceAssetType,
            quantity = availableItem.EnqueueCommand.Quantity,
            requestedAtUtc = "2026-06-08T12:15:00Z",
        });

        Assert.Equal(HttpStatusCode.Created, enqueueResponse.StatusCode);

        using var verificationScope = configuredFactory.Services.CreateScope();
        var verificationDb = verificationScope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        Assert.Equal(
            1,
            await verificationDb.Set<AssetProductionOrder>().CountAsync(x =>
                x.PlanetId == SeedOwnedPlanetId &&
                x.Status == AssetProductionOrderStatus.Cancelled));
    }

    [Fact]
    public async Task PrepareOrbitalQaStateIsIdempotentAndDoesNotMutateUnrelatedCivilizationState()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" }))
        {
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        }

        var otherCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000099");
        var foreignPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000099");

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
            dbContext.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(foreignPlanetId));
            dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(foreignPlanetId, otherCivilizationId));
            dbContext.Set<AssetProductionOrder>().Add(AssetProductionOrder.Create(
                foreignPlanetId,
                AssetProductionTarget.Orbital,
                null,
                SpaceAssetType.CargoCraft,
                1,
                90_000,
                new DateTime(2026, 6, 8, 12, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 6, 8, 12, 10, 0, DateTimeKind.Utc),
                AssetProductionOrderStatus.Active));
            await dbContext.SaveChangesAsync();
        }

        using var firstResponse = await client.PostAsJsonAsync("/api/dev/shipyard/qa-state/prepare", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId
        });
        var firstPayload = await firstResponse.Content.ReadFromJsonAsync<PrepareOrbitalQaStateResponse>();

        using var secondResponse = await client.PostAsJsonAsync("/api/dev/shipyard/qa-state/prepare", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId
        });
        var secondPayload = await secondResponse.Content.ReadFromJsonAsync<PrepareOrbitalQaStateResponse>();

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        Assert.NotNull(firstPayload);
        Assert.NotNull(secondPayload);
        Assert.True(firstPayload!.Succeeded);
        Assert.True(secondPayload!.Succeeded);
        Assert.Equal(0, firstPayload.BlockingOrdersAfter);
        Assert.Equal(0, secondPayload.BlockingOrdersBefore);
        Assert.Equal(0, secondPayload.BlockingOrdersAfter);

        using var verificationScope = configuredFactory.Services.CreateScope();
        var verificationDb = verificationScope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        Assert.Equal(
            1,
            await verificationDb.Set<AssetProductionOrder>().CountAsync(x =>
                x.PlanetId == foreignPlanetId &&
                x.Status == AssetProductionOrderStatus.Active));
    }

    [Fact]
    public async Task EnqueueReturnsBadRequestWhenCivilizationIdIsEmptyWithoutMutatingState()
    {
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"));
        using var client = configuredFactory.CreateClient();

        var before = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = Guid.Empty,
            planetId = SeedOwnedPlanetId,
            target = AssetProductionTarget.Orbital,
            spaceAssetType = SpaceAssetType.ScoutCraft,
            quantity = 1,
            requestedAtUtc = "2026-12-01T12:00:00Z",
        });
        var payload = await response.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();
        var after = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Equal(["Civilization id is required."], payload.Errors);
        AssertPlanetSnapshotUnchanged(before, after);
    }

    [Fact]
    public async Task EnqueueReturnsBadRequestWhenPlanetIdIsEmptyWithoutMutatingState()
    {
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"));
        using var client = configuredFactory.CreateClient();

        var before = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = Guid.Empty,
            target = AssetProductionTarget.Orbital,
            spaceAssetType = SpaceAssetType.ScoutCraft,
            quantity = 1,
            requestedAtUtc = "2026-12-01T12:00:00Z",
        });
        var payload = await response.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();
        var after = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Equal(["Planet id is required."], payload.Errors);
        AssertPlanetSnapshotUnchanged(before, after);
    }

    [Fact]
    public async Task EnqueueReturnsConflictWhenPlanetIsNotOwnedByRequestingCivilizationWithoutMutatingOwnedPlanetState()
    {
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"));
        using var client = configuredFactory.CreateClient();

        var ownedBefore = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);
        var foreignBefore = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOuterPlanetId);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOuterPlanetId,
            target = AssetProductionTarget.Orbital,
            spaceAssetType = SpaceAssetType.ScoutCraft,
            quantity = 1,
            requestedAtUtc = "2026-01-01T12:00:00Z",
        });
        var payload = await response.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();
        var ownedAfter = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);
        var foreignAfter = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOuterPlanetId);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Equal(["Planet is not owned by the requesting civilization."], payload.Errors);
        AssertPlanetSnapshotUnchanged(ownedBefore, ownedAfter);
        AssertPlanetSnapshotUnchanged(foreignBefore, foreignAfter);
    }

    [Fact]
    public async Task EnqueueReturnsConflictWhenResourcesAreInsufficientWithoutMutatingState()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName, context =>
        {
            var stockpile = context.PlanetResourceStockpiles.Single(x => x.PlanetId == SeedOwnedPlanetId);
            stockpile.Spend(0, 50, 30, 20);
        });
        using var client = CreateConfiguredClient(databaseName);
        var before = await CapturePlanetSnapshotAsync(dbContext, SeedOwnedPlanetId);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId,
            target = AssetProductionTarget.Orbital,
            spaceAssetType = SpaceAssetType.ScoutCraft,
            quantity = 1,
            requestedAtUtc = "2026-01-01T12:00:00Z",
        });
        var payload = await response.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();
        var after = await CapturePlanetSnapshotAsync(dbContext, SeedOwnedPlanetId);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Equal(["Insufficient resources."], payload.Errors);
        AssertPlanetSnapshotUnchanged(before, after);
    }

    [Fact]
    public async Task EnqueueReturnsConflictWhenRequirementIsMissingWithoutMutatingState()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName, context =>
        {
            var stockpile = context.PlanetResourceStockpiles.Single(x => x.PlanetId == SeedOwnedPlanetId);
            stockpile.Increase(ResourceType.Metal, 400);
            stockpile.Increase(ResourceType.Crystal, 200);
            stockpile.Increase(ResourceType.Gas, 120);
        });
        using var client = CreateConfiguredClient(databaseName);
        var before = await CapturePlanetSnapshotAsync(dbContext, SeedOwnedPlanetId);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId,
            target = AssetProductionTarget.Orbital,
            spaceAssetType = SpaceAssetType.EscortCraft,
            quantity = 1,
            requestedAtUtc = "2026-01-01T12:00:00Z",
        });
        var payload = await response.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();
        var after = await CapturePlanetSnapshotAsync(dbContext, SeedOwnedPlanetId);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Equal(["Required building is missing or below required level."], payload.Errors);
        AssertPlanetSnapshotUnchanged(before, after);
    }

    [Fact]
    public async Task EnqueueReturnsBadRequestWhenOrbitalAssetTypeIsMissingWithoutMutatingState()
    {
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"));
        using var client = configuredFactory.CreateClient();

        var before = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId,
            target = AssetProductionTarget.Orbital,
            quantity = 1,
            requestedAtUtc = "2026-01-01T12:00:00Z",
        });
        var payload = await response.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();
        var after = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Contains("Space asset type is required.", payload.Errors);
        AssertPlanetSnapshotUnchanged(before, after);
    }

    [Fact]
    public async Task EnqueueReturnsBadRequestWhenOrbitalAssetTypeIsInvalidWithoutMutatingState()
    {
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"));
        using var client = configuredFactory.CreateClient();

        var before = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);

        using var response = await client.PostAsJsonAsync("/api/dev/assets/production/enqueue", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId,
            target = AssetProductionTarget.Orbital,
            spaceAssetType = 999,
            quantity = 1,
            requestedAtUtc = "2026-01-01T12:00:00Z",
        });
        var payload = await response.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();
        var after = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload!.Succeeded);
        Assert.Contains("Space asset type is invalid.", payload.Errors);
        AssertPlanetSnapshotUnchanged(before, after);
    }

    [Fact]
    public async Task EnqueueReturnsConflictWhenOpenQueueAlreadyExistsWithoutAdditionalMutation()
    {
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"));
        using var client = configuredFactory.CreateClient();

        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        using var initialResponse = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var initialPayload = await initialResponse.Content.ReadFromJsonAsync<ShipyardUiStateEnvelope>();

        Assert.Equal(HttpStatusCode.OK, initialResponse.StatusCode);
        Assert.NotNull(initialPayload?.UiState?.Shipyard);

        var availableItem = Assert.Single(initialPayload.UiState.Shipyard.Catalog.Where(item => item.AssetType == SpaceAssetType.ScoutCraft));
        Assert.NotNull(availableItem.EnqueueCommand);

        using var firstEnqueueResponse = await client.PostAsJsonAsync(availableItem.EnqueueCommand.Route, new
        {
            civilizationId = availableItem.EnqueueCommand.CivilizationId,
            planetId = availableItem.EnqueueCommand.PlanetId,
            target = availableItem.EnqueueCommand.Target,
            spaceAssetType = availableItem.EnqueueCommand.SpaceAssetType,
            quantity = availableItem.EnqueueCommand.Quantity,
            requestedAtUtc = "2026-01-01T12:00:00Z",
        });
        var firstEnqueuePayload = await firstEnqueueResponse.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();

        Assert.Equal(HttpStatusCode.Created, firstEnqueueResponse.StatusCode);
        Assert.NotNull(firstEnqueuePayload);
        Assert.True(firstEnqueuePayload!.Succeeded);

        var afterFirstSuccess = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);

        using var secondEnqueueResponse = await client.PostAsJsonAsync(availableItem.EnqueueCommand.Route, new
        {
            civilizationId = availableItem.EnqueueCommand.CivilizationId,
            planetId = availableItem.EnqueueCommand.PlanetId,
            target = availableItem.EnqueueCommand.Target,
            spaceAssetType = availableItem.EnqueueCommand.SpaceAssetType,
            quantity = availableItem.EnqueueCommand.Quantity,
            requestedAtUtc = "2026-01-01T12:01:00Z",
        });
        var secondEnqueuePayload = await secondEnqueueResponse.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();
        var afterSecondAttempt = await CapturePlanetSnapshotAsync(configuredFactory.Services, SeedOwnedPlanetId);

        Assert.Equal(HttpStatusCode.Conflict, secondEnqueueResponse.StatusCode);
        Assert.NotNull(secondEnqueuePayload);
        Assert.False(secondEnqueuePayload!.Succeeded);
        Assert.Equal(["Planet already has an open asset production order."], secondEnqueuePayload.Errors);
        AssertPlanetSnapshotUnchanged(afterFirstSuccess, afterSecondAttempt);
    }

    [Fact]
    public async Task EnqueueReturnsCreatedAndRefreshesShipyardQueueForSeededSuccessPath()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        using var initialResponse = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var initialPayload = await initialResponse.Content.ReadFromJsonAsync<ShipyardUiStateEnvelope>();

        Assert.Equal(HttpStatusCode.OK, initialResponse.StatusCode);
        Assert.NotNull(initialPayload?.UiState?.Shipyard);

        var availableItem = Assert.Single(initialPayload.UiState.Shipyard.Catalog.Where(item => item.AssetType == SpaceAssetType.ScoutCraft));
        Assert.NotNull(availableItem.EnqueueCommand);
        var resourcesBefore = initialPayload.UiState.Shipyard.ResourceStockpile.ToDictionary(x => x.ResourceType, x => x.Quantity);
        var queueCountBefore = initialPayload.UiState.Shipyard.Queue.Count;

        using var enqueueResponse = await client.PostAsJsonAsync(availableItem.EnqueueCommand.Route, new
        {
            civilizationId = availableItem.EnqueueCommand.CivilizationId,
            planetId = availableItem.EnqueueCommand.PlanetId,
            target = availableItem.EnqueueCommand.Target,
            spaceAssetType = availableItem.EnqueueCommand.SpaceAssetType,
            quantity = availableItem.EnqueueCommand.Quantity,
            requestedAtUtc = "2026-12-01T12:00:00Z",
        });
        var enqueuePayload = await enqueueResponse.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();

        Assert.Equal(HttpStatusCode.Created, enqueueResponse.StatusCode);
        Assert.NotNull(enqueuePayload);
        Assert.True(enqueuePayload!.Succeeded);
        Assert.NotNull(enqueuePayload.OrderId);

        using var followUpResponse = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var followUpPayload = await followUpResponse.Content.ReadFromJsonAsync<ShipyardUiStateEnvelope>();

        Assert.Equal(HttpStatusCode.OK, followUpResponse.StatusCode);
        Assert.NotNull(followUpPayload?.UiState?.Shipyard);
        Assert.Equal(queueCountBefore + 1, followUpPayload.UiState.Shipyard.Queue.Count);
        Assert.Contains(
            followUpPayload.UiState.Shipyard.Queue,
            item => item.OrderId == enqueuePayload.OrderId &&
                item.AssetType == availableItem.AssetType &&
                item.Quantity == availableItem.EnqueueCommand.Quantity &&
                item.Status == AssetProductionOrderStatus.Active);
        Assert.False(followUpPayload.UiState.Shipyard.ActionSummary.EnqueueSupported);
        Assert.Contains(
            followUpPayload.UiState.Shipyard.Catalog,
            item => item.AssetType == SpaceAssetType.ScoutCraft && item.AvailabilityReason == "OpenProductionOrderExists");

        foreach (var cost in availableItem.Cost)
        {
            var before = resourcesBefore[cost.ResourceType];
            var after = followUpPayload.UiState.Shipyard.ResourceStockpile.Single(x => x.ResourceType == cost.ResourceType).Quantity;
            Assert.Equal(before - cost.Quantity, after);
        }

        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var persistedOrder = await dbContext.Set<AssetProductionOrder>()
            .SingleAsync(x => x.Id == enqueuePayload.OrderId!.Value);

        Assert.Equal(SeedOwnedPlanetId, persistedOrder.PlanetId);
        Assert.Equal(AssetProductionTarget.Orbital, persistedOrder.Target);
        Assert.Equal(availableItem.AssetType, persistedOrder.SpaceAssetType);
        Assert.Equal(availableItem.EnqueueCommand.Quantity, persistedOrder.Quantity);
        Assert.Equal(AssetProductionOrderStatus.Active, persistedOrder.Status);
    }

    [Fact]
    public async Task CockpitValidationReapplyPreservesManualShipyardOrderAndKeepsUiStateReadable()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using var firstSeedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        Assert.Equal(HttpStatusCode.OK, firstSeedResponse.StatusCode);

        using var initialResponse = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var initialPayload = await initialResponse.Content.ReadFromJsonAsync<ShipyardUiStateEnvelope>();

        Assert.Equal(HttpStatusCode.OK, initialResponse.StatusCode);
        Assert.NotNull(initialPayload?.UiState?.Shipyard);

        var availableItem = Assert.Single(initialPayload.UiState.Shipyard.Catalog.Where(item => item.AssetType == SpaceAssetType.ScoutCraft));
        Assert.NotNull(availableItem.EnqueueCommand);

        using var enqueueResponse = await client.PostAsJsonAsync(availableItem.EnqueueCommand.Route, new
        {
            civilizationId = availableItem.EnqueueCommand.CivilizationId,
            planetId = availableItem.EnqueueCommand.PlanetId,
            target = availableItem.EnqueueCommand.Target,
            spaceAssetType = availableItem.EnqueueCommand.SpaceAssetType,
            quantity = availableItem.EnqueueCommand.Quantity,
            requestedAtUtc = "2026-12-01T12:00:00Z",
        });
        var enqueuePayload = await enqueueResponse.Content.ReadFromJsonAsync<ShipyardEnqueueResponse>();

        Assert.Equal(HttpStatusCode.Created, enqueueResponse.StatusCode);
        Assert.NotNull(enqueuePayload);
        Assert.True(enqueuePayload!.Succeeded);
        Assert.NotNull(enqueuePayload.OrderId);

        using var reseedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "cockpit-validation" });
        Assert.Equal(HttpStatusCode.OK, reseedResponse.StatusCode);

        using var followUpResponse = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var followUpPayload = await followUpResponse.Content.ReadFromJsonAsync<ShipyardUiStateEnvelope>();

        Assert.Equal(HttpStatusCode.OK, followUpResponse.StatusCode);
        Assert.NotNull(followUpPayload?.UiState?.Shipyard);
        Assert.Contains(followUpPayload.UiState.Shipyard.Queue, item => item.OrderId == enqueuePayload.OrderId);

        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();

        Assert.Equal(3, await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == SeedOwnedPlanetId));
        Assert.Equal(1, await dbContext.Set<AssetProductionOrder>().CountAsync(x =>
            x.PlanetId == SeedOwnedPlanetId &&
            x.Id == enqueuePayload.OrderId!.Value &&
            x.Status == AssetProductionOrderStatus.Active));
        Assert.Equal(1, await dbContext.Set<AssetProductionOrder>().CountAsync(x =>
            x.PlanetId == SeedOwnedPlanetId &&
            x.Target == AssetProductionTarget.Orbital &&
            x.SpaceAssetType == SpaceAssetType.ScoutCraft &&
            x.Status == AssetProductionOrderStatus.Completed));
        Assert.Equal(1, await dbContext.Set<AssetProductionOrder>().CountAsync(x =>
            x.PlanetId == SeedOwnedPlanetId &&
            x.Target == AssetProductionTarget.Planetary &&
            x.PlanetaryAssetType == PlanetaryAssetType.PatrolGroup &&
            x.Status == AssetProductionOrderStatus.Completed));
        Assert.Equal(2, await dbContext.Set<AssetProductionOrder>().CountAsync(x =>
            x.PlanetId == SeedOwnedPlanetId &&
            x.Status == AssetProductionOrderStatus.Completed));
    }

    private HttpClient CreateConfiguredClient(string databaseName) =>
        factory.WithInMemoryPersistence(databaseName: databaseName).CreateClient();

    private static async Task<PlanetMutationSnapshot> CapturePlanetSnapshotAsync(IServiceProvider services, Guid planetId)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        return await CapturePlanetSnapshotAsync(dbContext, planetId);
    }

    private static async Task<PlanetMutationSnapshot> CapturePlanetSnapshotAsync(VoidEmpiresDbContext dbContext, Guid planetId)
    {
        var stockpile = await dbContext.PlanetResourceStockpiles
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.PlanetId == planetId);
        var orders = await dbContext.Set<AssetProductionOrder>()
            .AsNoTracking()
            .Where(x => x.PlanetId == planetId)
            .OrderBy(x => x.Sequence)
            .Select(x => new PlanetOrderSnapshot(x.Id, x.Target, x.SpaceAssetType, x.Quantity, x.Status, x.Sequence))
            .ToArrayAsync();

        return new PlanetMutationSnapshot(
            planetId,
            stockpile?.Credits,
            stockpile?.Metal,
            stockpile?.Crystal,
            stockpile?.Gas,
            orders);
    }

    private static void AssertPlanetSnapshotUnchanged(PlanetMutationSnapshot before, PlanetMutationSnapshot after)
    {
        Assert.Equal(before.PlanetId, after.PlanetId);
        Assert.Equal(before.Credits, after.Credits);
        Assert.Equal(before.Metal, after.Metal);
        Assert.Equal(before.Crystal, after.Crystal);
        Assert.Equal(before.Gas, after.Gas);
        Assert.Equal(before.Orders, after.Orders);
    }

    private static VoidEmpiresDbContext CreateSeededDbContext(string databaseName, Action<VoidEmpiresDbContext>? seedOverride = null)
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options);

        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation")).GetAwaiter().GetResult();
        seedOverride?.Invoke(dbContext);
        dbContext.SaveChanges();

        return dbContext;
    }

    private sealed record ShipyardEnqueueResponse(
        bool Succeeded,
        Guid? OrderId,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
        string[] Errors);

    private sealed record PrepareOrbitalQaStateResponse(
        bool Succeeded,
        int BlockingOrdersBefore,
        int BlockingOrdersAfter,
        PrepareOrbitalQaStateResourceState? ResourcesBefore,
        PrepareOrbitalQaStateResourceState? ResourcesAfter,
        string[] Notes,
        string[] Errors);

    private sealed record PrepareOrbitalQaStateResourceState(
        decimal Credits,
        decimal Metal,
        decimal Crystal,
        decimal Gas);

    private sealed record ShipyardUiStateEnvelope(
        bool Succeeded,
        ShipyardUiStatePayload? UiState,
        string[] Errors);

    private sealed record ShipyardUiStatePayload(
        Guid CivilizationId,
        Guid? SelectedPlanetId,
        ShipyardPlanetPayload? Shipyard,
        string[] Errors);

    private sealed record ShipyardPlanetPayload(
        Guid PlanetId,
        string PlanetName,
        IReadOnlyList<ShipyardResourceBalancePayload> ResourceStockpile,
        ShipyardActionSummaryPayload ActionSummary,
        IReadOnlyList<ShipyardQueueItemPayload> Queue,
        IReadOnlyList<ShipyardCatalogItemPayload> Catalog);

    private sealed record ShipyardResourceBalancePayload(
        ResourceType ResourceType,
        decimal Quantity);

    private sealed record ShipyardActionSummaryPayload(
        string QueueActionStatus,
        string QueueActionReason,
        bool EnqueueSupported,
        string EnqueueActionStatus,
        string EnqueueActionReason,
        bool CompleteDueSupported,
        string CompleteDueActionStatus,
        string CompleteDueActionReason,
        int OpenQueueCount,
        int DueQueueCount);

    private sealed record ShipyardQueueItemPayload(
        Guid OrderId,
        SpaceAssetType AssetType,
        int Quantity,
        int Sequence,
        AssetProductionOrderStatus Status,
        bool IsDue);

    private sealed record ShipyardCatalogItemPayload(
        SpaceAssetType AssetType,
        string AvailabilityStatus,
        string AvailabilityReason,
        IReadOnlyList<ShipyardResourceBalancePayload> Cost,
        ShipyardEnqueueCommandPayload? EnqueueCommand);

    private sealed record ShipyardEnqueueCommandPayload(
        string ActionKey,
        string Method,
        string Route,
        Guid CivilizationId,
        Guid PlanetId,
        AssetProductionTarget Target,
        SpaceAssetType SpaceAssetType,
        int Quantity);

    private sealed record PlanetMutationSnapshot(
        Guid PlanetId,
        decimal? Credits,
        decimal? Metal,
        decimal? Crystal,
        decimal? Gas,
        IReadOnlyList<PlanetOrderSnapshot> Orders);

    private sealed record PlanetOrderSnapshot(
        Guid OrderId,
        AssetProductionTarget Target,
        SpaceAssetType? SpaceAssetType,
        int Quantity,
        AssetProductionOrderStatus Status,
        int Sequence);
}
