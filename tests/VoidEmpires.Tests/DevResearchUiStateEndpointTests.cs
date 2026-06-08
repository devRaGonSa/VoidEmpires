using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VoidEmpires.Application.Development;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevResearchUiStateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private const string SeedCivilizationId = "00000000-0000-0000-0000-000000000001";
    private const string SeedOwnedPlanetId = "40000000-0000-0000-0000-000000000001";
    private static readonly InMemoryDatabaseRoot SharedDatabaseRoot = new();

    [Fact]
    public async Task UiStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();
        using var response = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PrepareResearchQaStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();
        using var response = await client.PostAsJsonAsync("/api/dev/research/qa-state/prepare", new { });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UiStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();
        using var response = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task PrepareResearchQaStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();
        using var response = await client.PostAsJsonAsync("/api/dev/research/qa-state/prepare", new { });
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task PrepareResearchQaStateCancelsOpenOrdersAndRestoresEnqueueReadiness()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "research-validation" }))
        {
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        }

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
            var civilizationId = Guid.Parse(SeedCivilizationId);
            var planetId = Guid.Parse(SeedOwnedPlanetId);
            var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == planetId);
            dbContext.Entry(stockpile).Property(x => x.Credits).CurrentValue = 0m;
            dbContext.Entry(stockpile).Property(x => x.Metal).CurrentValue = 0m;
            dbContext.Entry(stockpile).Property(x => x.Crystal).CurrentValue = 0m;
            dbContext.Entry(stockpile).Property(x => x.Gas).CurrentValue = 0m;

            var sequence = await dbContext.ResearchOrders
                .Where(x => x.CivilizationId == civilizationId)
                .Select(x => (int?)x.Sequence)
                .MaxAsync() ?? 0;

            dbContext.ResearchOrders.Add(ResearchOrder.Create(
                civilizationId,
                planetId,
                ResearchType.ResourceExtraction,
                1,
                sequence + 1,
                new DateTime(2026, 6, 8, 12, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 6, 8, 12, 10, 0, DateTimeKind.Utc),
                ResearchQueueItemStatus.Active));

            await dbContext.SaveChangesAsync();
        }

        using var response = await client.PostAsJsonAsync("/api/dev/research/qa-state/prepare", new { });
        var payload = await response.Content.ReadFromJsonAsync<PrepareResearchQaStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
        Assert.Equal(1, payload.BlockingOrdersBefore);
        Assert.Equal(0, payload.BlockingOrdersAfter);
        Assert.NotNull(payload.ResourcesAfter);
        Assert.True(payload.ResourcesAfter!.Credits >= 125m);
        Assert.True(payload.ResourcesAfter.Metal >= 110m);
        Assert.True(payload.ResourcesAfter.Crystal >= 70m);
        Assert.True(payload.ResourcesAfter.Gas >= 30m);

        using var uiStateResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var uiStatePayload = await uiStateResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, uiStateResponse.StatusCode);
        Assert.NotNull(uiStatePayload?.UiState);
        var availableResearch = Assert.Single(uiStatePayload.UiState.TechnologyHints.Where(x => x.CanEnqueue));

        using var enqueueResponse = await client.PostAsJsonAsync(
            "/api/dev/research/orders/enqueue",
            new
            {
                civilizationId = SeedCivilizationId,
                sourcePlanetId = SeedOwnedPlanetId,
                researchType = availableResearch.ResearchType.ToString(),
                requestedAtUtc = "2026-06-08T12:15:00Z"
            });

        Assert.Equal(HttpStatusCode.Created, enqueueResponse.StatusCode);

        using var verificationScope = configuredFactory.Services.CreateScope();
        var verificationDb = verificationScope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        Assert.Equal(
            1,
            await verificationDb.ResearchOrders.CountAsync(x =>
                x.CivilizationId == Guid.Parse(SeedCivilizationId) &&
                x.Status == ResearchQueueItemStatus.Cancelled));
    }

    [Fact]
    public async Task PrepareResearchQaStateIsIdempotentAndDoesNotMutateUnrelatedCivilizations()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "research-validation" }))
        {
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        }

        var otherCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000099");

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
            var foreignOrder = ResearchOrder.Create(
                otherCivilizationId,
                Guid.Parse(SeedOwnedPlanetId),
                ResearchType.EnergySystems,
                1,
                90_000,
                new DateTime(2026, 6, 8, 12, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 6, 8, 12, 10, 0, DateTimeKind.Utc),
                ResearchQueueItemStatus.Active);
            dbContext.ResearchOrders.Add(foreignOrder);
            await dbContext.SaveChangesAsync();
        }

        using var firstResponse = await client.PostAsJsonAsync("/api/dev/research/qa-state/prepare", new
        {
            civilizationId = SeedCivilizationId,
            sourcePlanetId = SeedOwnedPlanetId
        });
        var firstPayload = await firstResponse.Content.ReadFromJsonAsync<PrepareResearchQaStateResponse>();

        using var secondResponse = await client.PostAsJsonAsync("/api/dev/research/qa-state/prepare", new
        {
            civilizationId = SeedCivilizationId,
            planetId = SeedOwnedPlanetId
        });
        var secondPayload = await secondResponse.Content.ReadFromJsonAsync<PrepareResearchQaStateResponse>();

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
            await verificationDb.ResearchOrders.CountAsync(x =>
                x.CivilizationId == otherCivilizationId &&
                x.Status == ResearchQueueItemStatus.Active));
        Assert.Equal(
            1,
            await verificationDb.ResearchOrders.CountAsync(x =>
                x.CivilizationId == Guid.Parse(SeedCivilizationId) &&
                x.Status == ResearchQueueItemStatus.Completed));
    }

    [Fact]
    public async Task UiStateReturnsBadRequestWhenCivilizationIdIsMissing()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var client = CreateConfiguredClient(databaseName);
        using var response = await client.GetAsync("/api/dev/research/ui-state");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UiStateReturnsNotFoundForUnknownCivilizationAndPlanet()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName);
        using var client = CreateConfiguredClient(databaseName);

        using var missingCivilizationResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={Guid.NewGuid()}");
        using var missingPlanetResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, missingCivilizationResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missingPlanetResponse.StatusCode);
    }

    [Fact]
    public async Task UiStateReturnsCatalogQueueAndProjectsWithoutMutatingState()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName);
        var civilizationId = Guid.Parse(SeedCivilizationId);
        var planetId = Guid.Parse(SeedOwnedPlanetId);
        var stockpile = dbContext.PlanetResourceStockpiles.Single(x => x.PlanetId == planetId);
        stockpile.Increase(ResourceType.Metal, 500);
        stockpile.Increase(ResourceType.Crystal, 500);
        dbContext.ResearchProjects.Add(ResearchProject.Create(civilizationId, ResearchType.ResourceExtraction));
        dbContext.ResearchOrders.Add(ResearchOrder.Create(
            civilizationId,
            planetId,
            ResearchType.PlanetaryEngineering,
            1,
            1,
            new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 1, 1, 12, 20, 0, DateTimeKind.Utc),
            ResearchQueueItemStatus.Active));
        await dbContext.SaveChangesAsync();

        var queueBefore = await dbContext.ResearchOrders.CountAsync();
        var projectsBefore = await dbContext.ResearchProjects.CountAsync();
        var metalBefore = stockpile.Metal;

        using var client = CreateConfiguredClient(databaseName);
        using var response = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
        Assert.NotNull(payload.UiState);
        Assert.Equal(Guid.Parse(SeedOwnedPlanetId), payload.UiState.SelectedPlanetId);
        Assert.Equal("Aurelia", payload.UiState.SelectedPlanetName);
        Assert.Equal(8, payload.UiState.Catalog.Length);
        Assert.Single(payload.UiState.Queue);
        Assert.Single(payload.UiState.Projects);
        Assert.Contains(payload.UiState.TechnologyHints, x => x.ResearchType == ResearchType.PlanetaryEngineering && x.StatusKey == "InResearch" && !x.CanEnqueue);
        Assert.Equal(queueBefore, await dbContext.ResearchOrders.CountAsync());
        Assert.Equal(projectsBefore, await dbContext.ResearchProjects.CountAsync());
        Assert.Equal(metalBefore, stockpile.Metal);
    }

    [Fact]
    public async Task UiStateExposesAvailableAndBlockedResearchFromMinimalValidationSeed()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName);
        using var client = CreateConfiguredClient(databaseName);

        using var response = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
        Assert.NotNull(payload.UiState);
        Assert.Empty(payload.UiState.Queue);
        Assert.Empty(payload.UiState.Projects);
        Assert.Contains(
            payload.UiState.TechnologyHints,
            item => item.ResearchType == ResearchType.PlanetaryEngineering &&
                item.CanEnqueue &&
                item.StatusKey == "Available" &&
                item.EnqueueCommand is not null &&
                item.EnqueueCommand.ActionKey == "research.order.enqueue" &&
                item.EnqueueCommand.Method == "POST" &&
                item.EnqueueCommand.Route == "/api/dev/research/orders/enqueue" &&
                item.EnqueueCommand.CivilizationId == Guid.Parse(SeedCivilizationId) &&
                item.EnqueueCommand.SourcePlanetId == Guid.Parse(SeedOwnedPlanetId) &&
                item.EnqueueCommand.ResearchType == ResearchType.PlanetaryEngineering &&
                item.EnqueueCommand.TargetLevel == 1);
        Assert.Contains(
            payload.UiState.TechnologyHints,
            item => item.ResearchType == ResearchType.ResourceExtraction &&
                item.CanEnqueue &&
                item.StatusKey == "Available");
        Assert.Contains(
            payload.UiState.TechnologyHints,
            item => item.ResearchType == ResearchType.EnergySystems &&
                item.CanEnqueue &&
                item.StatusKey == "Available");
        Assert.Contains(
            payload.UiState.TechnologyHints,
            item => item.ResearchType == ResearchType.ConstructionAutomation &&
                !item.CanEnqueue &&
                item.StatusKey == "InsufficientResources");
    }

    [Fact]
    public async Task ReapplyingMinimalValidationSeedRestoresResearchAvailabilityAfterStockpileConsumption()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName);
        var planetId = Guid.Parse(SeedOwnedPlanetId);
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == planetId);
        stockpile.Spend(stockpile.Credits, 70, 60, 40);
        await dbContext.SaveChangesAsync();

        using (var client = CreateConfiguredClient(databaseName))
        using (var depletedResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}"))
        {
            var depletedPayload = await depletedResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();
            Assert.Equal(HttpStatusCode.OK, depletedResponse.StatusCode);
            Assert.NotNull(depletedPayload);
            Assert.NotNull(depletedPayload!.UiState);
            Assert.DoesNotContain(
                depletedPayload.UiState.TechnologyHints,
                item => item.ResearchType == ResearchType.PlanetaryEngineering && item.CanEnqueue);
        }

        await new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));

        using var refreshedClient = CreateConfiguredClient(databaseName);
        using var refreshedResponse = await refreshedClient.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var refreshedPayload = await refreshedResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, refreshedResponse.StatusCode);
        Assert.NotNull(refreshedPayload);
        Assert.NotNull(refreshedPayload!.UiState);
        Assert.Contains(
            refreshedPayload.UiState.TechnologyHints,
            item => item.ResearchType == ResearchType.PlanetaryEngineering &&
                item.CanEnqueue &&
                item.StatusKey == "Available");
    }

    [Fact]
    public async Task MinimalValidationResearchFlowEnqueueUpdatesQueueAndLeavesBlockedItemsBlocked()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName);
        var planetId = Guid.Parse(SeedOwnedPlanetId);
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == planetId);
        var metalBeforeRead = stockpile.Metal;
        var crystalBeforeRead = stockpile.Crystal;

        using var client = CreateConfiguredClient(databaseName);

        using var initialResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var initialPayload = await initialResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, initialResponse.StatusCode);
        Assert.NotNull(initialPayload?.UiState);
        Assert.Equal(metalBeforeRead, stockpile.Metal);
        Assert.Equal(crystalBeforeRead, stockpile.Crystal);
        Assert.True(initialPayload.UiState.TechnologyHints.Count(x => x.CanEnqueue) >= 1);
        Assert.True(initialPayload.UiState.TechnologyHints.Count(x => !x.CanEnqueue && !x.CanCompleteDue) >= 1);

        var availableResearch = Assert.Single(initialPayload.UiState.TechnologyHints.Where(x => x.ResearchType == ResearchType.PlanetaryEngineering));
        Assert.True(availableResearch.CanEnqueue);

        Assert.NotNull(availableResearch.EnqueueCommand);

        using var enqueueResponse = await client.PostAsJsonAsync(
            availableResearch.EnqueueCommand!.Route,
            new
            {
                civilizationId = availableResearch.EnqueueCommand.CivilizationId,
                sourcePlanetId = availableResearch.EnqueueCommand.SourcePlanetId,
                researchType = availableResearch.EnqueueCommand.ResearchType.ToString(),
                requestedAtUtc = "2026-01-01T12:00:00Z"
            });
        var enqueuePayload = await enqueueResponse.Content.ReadFromJsonAsync<EnqueueResearchOrderApiResponse>();

        Assert.Equal(HttpStatusCode.Created, enqueueResponse.StatusCode);
        Assert.NotNull(enqueuePayload);
        Assert.True(enqueuePayload!.Succeeded);
        Assert.NotNull(enqueuePayload.OrderId);

        using var followUpResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var followUpPayload = await followUpResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, followUpResponse.StatusCode);
        Assert.NotNull(followUpPayload?.UiState);
        Assert.Single(followUpPayload.UiState.Queue);
        Assert.Contains(
            followUpPayload.UiState.TechnologyHints,
            item => item.ResearchType == ResearchType.PlanetaryEngineering &&
                !item.CanEnqueue &&
                item.StatusKey == "InResearch");
        Assert.Contains(
            followUpPayload.UiState.TechnologyHints,
            item => item.ResearchType == ResearchType.ResourceExtraction &&
                !item.CanEnqueue &&
                item.StatusKey == "InsufficientResources");
        Assert.Contains(
            followUpPayload.UiState.TechnologyHints,
            item => item.ResearchType == ResearchType.EnergySystems &&
                !item.CanEnqueue &&
                item.StatusKey == "InsufficientResources");
        Assert.Equal(0, followUpPayload.UiState.TechnologyHints.Count(x => x.CanEnqueue));
    }

    [Fact]
    public async Task ResearchValidationProfileReturnsRicherCatalogWithoutBreakingPrimaryEnqueuePath()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName, "research-validation");
        using var client = CreateConfiguredClient(databaseName);

        using var initialResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var initialPayload = await initialResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, initialResponse.StatusCode);
        Assert.NotNull(initialPayload?.UiState);
        Assert.Empty(initialPayload.UiState.Queue.Where(x => x.Status is ResearchQueueItemStatus.Pending or ResearchQueueItemStatus.Active));
        Assert.Single(initialPayload.UiState.Projects, x => x.ResearchType == ResearchType.EnergySystems && x.Level == 1);
        Assert.Single(initialPayload.UiState.Queue, x => x.ResearchType == ResearchType.EnergySystems && x.Status == ResearchQueueItemStatus.Completed);
        Assert.Single(initialPayload.UiState.TechnologyHints.Where(x => x.CanEnqueue));
        Assert.True(initialPayload.UiState.TechnologyHints.Count(x => !x.CanEnqueue) >= 3);
        Assert.All(
            initialPayload.UiState.TechnologyHints.Where(x => !x.CanEnqueue),
            item => Assert.Equal("InsufficientResources", item.StatusKey));

        var availableResearch = Assert.Single(initialPayload.UiState.TechnologyHints.Where(x => x.CanEnqueue));
        Assert.Equal(ResearchType.PlanetaryEngineering, availableResearch.ResearchType);

        using var enqueueResponse = await client.PostAsJsonAsync(
            availableResearch.EnqueueCommand!.Route,
            new
            {
                civilizationId = availableResearch.EnqueueCommand.CivilizationId,
                sourcePlanetId = availableResearch.EnqueueCommand.SourcePlanetId,
                researchType = availableResearch.EnqueueCommand.ResearchType.ToString(),
                requestedAtUtc = "2026-01-01T12:00:00Z"
            });

        Assert.Equal(HttpStatusCode.Created, enqueueResponse.StatusCode);

        using var followUpResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var followUpPayload = await followUpResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, followUpResponse.StatusCode);
        Assert.NotNull(followUpPayload?.UiState);
        Assert.Single(followUpPayload.UiState.Queue.Where(x => x.Status is ResearchQueueItemStatus.Pending or ResearchQueueItemStatus.Active));
        Assert.Contains(
            followUpPayload.UiState.TechnologyHints,
            item => item.ResearchType == ResearchType.PlanetaryEngineering &&
                !item.CanEnqueue &&
                item.StatusKey == "InResearch");
    }

    [Fact]
    public async Task CockpitValidationProfileReturnsAvailableBlockedAndCompletedResearchHistory()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName, "cockpit-validation");
        using var client = CreateConfiguredClient(databaseName);

        using var response = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState);
        Assert.Single(payload.UiState.Queue, x => x.ResearchType == ResearchType.EnergySystems && x.Status == ResearchQueueItemStatus.Completed);
        Assert.Single(payload.UiState.Projects, x => x.ResearchType == ResearchType.EnergySystems && x.Level == 1);
        Assert.True(payload.UiState.TechnologyHints.Count(x => x.CanEnqueue) >= 1);
        Assert.True(payload.UiState.TechnologyHints.Count(x => !x.CanEnqueue) >= 1);
        Assert.Contains(payload.UiState.TechnologyHints, x => x.ResearchType == ResearchType.PlanetaryEngineering && x.CanEnqueue);
    }

    [Fact]
    public async Task ReapplyingResearchValidationSeedDoesNotDuplicateCompletedResearchHistory()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName, "research-validation");
        var service = new DevelopmentSeedService(dbContext);

        await service.ApplyAsync(new ApplyDevelopmentSeedRequest("research-validation"));

        Assert.Equal(1, await dbContext.ResearchProjects.CountAsync(x => x.CivilizationId == Guid.Parse(SeedCivilizationId) && x.ResearchType == ResearchType.EnergySystems));
        Assert.Equal(1, await dbContext.ResearchOrders.CountAsync(x =>
            x.CivilizationId == Guid.Parse(SeedCivilizationId) &&
            x.SourcePlanetId == Guid.Parse(SeedOwnedPlanetId) &&
            x.ResearchType == ResearchType.EnergySystems &&
            x.Status == ResearchQueueItemStatus.Completed));
    }

    [Fact]
    public async Task BlockedResearchCommandMetadataRejectsWithExpectedReason()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName);
        using var client = CreateConfiguredClient(databaseName);

        using var response = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState);

        var blockedResearch = Assert.Single(payload.UiState.TechnologyHints.Where(x =>
            x.ResearchType == ResearchType.ConstructionAutomation &&
            !x.CanEnqueue &&
            x.StatusKey == "InsufficientResources"));

        Assert.NotNull(blockedResearch.EnqueueCommand);

        using var enqueueResponse = await client.PostAsJsonAsync(
            blockedResearch.EnqueueCommand!.Route,
            new
            {
                civilizationId = blockedResearch.EnqueueCommand.CivilizationId,
                sourcePlanetId = blockedResearch.EnqueueCommand.SourcePlanetId,
                researchType = blockedResearch.EnqueueCommand.ResearchType.ToString(),
                requestedAtUtc = "2026-01-01T12:00:00Z"
            });
        var enqueuePayload = await enqueueResponse.Content.ReadFromJsonAsync<EnqueueResearchOrderApiResponse>();

        Assert.Equal(HttpStatusCode.Conflict, enqueueResponse.StatusCode);
        Assert.NotNull(enqueuePayload);
        Assert.False(enqueuePayload!.Succeeded);
        Assert.Equal(["Insufficient resources."], enqueuePayload.Errors);
    }

    [Fact]
    public async Task EnqueueResearchAcceptsFrontendStringEnumPayload()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        await using var dbContext = CreateSeededDbContext(databaseName);
        using var client = CreateConfiguredClient(databaseName);

        using var response = await client.PostAsJsonAsync(
            "/api/dev/research/orders/enqueue",
            new
            {
                civilizationId = SeedCivilizationId,
                sourcePlanetId = SeedOwnedPlanetId,
                researchType = "PlanetaryEngineering",
                requestedAtUtc = "2026-01-01T12:00:00Z"
            });
        var payload = await response.Content.ReadFromJsonAsync<EnqueueResearchOrderApiResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
        Assert.NotNull(payload.OrderId);
    }

    private HttpClient CreateConfiguredClient(string databaseName) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = $"Host=localhost;Database={databaseName}"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<VoidEmpiresDbContext>>();
                services.RemoveAll<VoidEmpiresDbContext>();
                services.AddDbContext<VoidEmpiresDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName, SharedDatabaseRoot));
            });
        }).CreateClient();

    private static VoidEmpiresDbContext CreateSeededDbContext(string databaseName, string profile = "minimal-validation")
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(databaseName, SharedDatabaseRoot)
            .Options);
        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest(profile)).GetAwaiter().GetResult();
        return dbContext;
    }

    private sealed record DevResearchUiStateResponse(
        bool Succeeded,
        DevResearchUiStateResult? UiState,
        string[] Errors);

    private sealed record DevResearchUiStateResult(
        Guid CivilizationId,
        Guid? SelectedPlanetId,
        string? SelectedPlanetName,
        ResearchDefinition[] Catalog,
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

    private sealed record EnqueueResearchOrderApiRequest(
        Guid? CivilizationId,
        Guid? SourcePlanetId,
        ResearchType? ResearchType,
        DateTime? RequestedAtUtc);

    private sealed record EnqueueResearchOrderApiResponse(
        bool Succeeded,
        Guid? OrderId,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
        IReadOnlyList<string> Errors);

    private sealed record PrepareResearchQaStateResponse(
        bool Succeeded,
        ResearchQaStatePreparationResult? Result,
        int BlockingOrdersBefore,
        int BlockingOrdersAfter,
        ResearchQaStatePreparationResourceState? ResourcesBefore,
        ResearchQaStatePreparationResourceState? ResourcesAfter,
        IReadOnlyList<string> Notes,
        IReadOnlyList<string> Errors);
}
