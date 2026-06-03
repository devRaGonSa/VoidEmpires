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
                item.StatusKey == "Available");
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
        ResearchCost EstimatedCost,
        IReadOnlyList<string> RequirementKeys);
}
