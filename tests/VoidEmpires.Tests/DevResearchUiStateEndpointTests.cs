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
    public async Task UiStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();
        using var response = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
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
                !item.CanEnqueue &&
                item.StatusKey == "InsufficientResources");
        Assert.Contains(
            payload.UiState.TechnologyHints,
            item => item.ResearchType == ResearchType.EnergySystems &&
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
        stockpile.Spend(stockpile.Credits, 25, 10, stockpile.Gas);
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

        var availableResearch = Assert.Single(initialPayload.UiState.TechnologyHints.Where(x => x.CanEnqueue));
        Assert.Equal(ResearchType.PlanetaryEngineering, availableResearch.ResearchType);

        using var enqueueResponse = await client.PostAsJsonAsync(
            "/api/dev/research/orders/enqueue",
            new EnqueueResearchOrderApiRequest(
                Guid.Parse(SeedCivilizationId),
                Guid.Parse(SeedOwnedPlanetId),
                availableResearch.ResearchType,
                new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc)));
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

    private static VoidEmpiresDbContext CreateSeededDbContext(string databaseName)
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(databaseName, SharedDatabaseRoot)
            .Options);
        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation")).GetAwaiter().GetResult();
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
}
