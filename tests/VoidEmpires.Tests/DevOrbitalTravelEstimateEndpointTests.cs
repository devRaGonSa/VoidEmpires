using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevOrbitalTravelEstimateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeededCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeededOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeededDestinationPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");
    private static readonly Guid CivilizationId = Guid.Parse("3659791b-71b4-4582-b9cd-ee396930d075");
    private static readonly Guid OrbitalGroupId = Guid.Parse("4a332c10-5a76-407e-8fb5-d1a29ad568bc");
    private static readonly Guid CurrentPlanetId = Guid.Parse("4813dab0-0de6-454c-8c6c-d70944b87bfa");
    private static readonly Guid DestinationPlanetId = Guid.Parse("0dd85c9f-7c1d-4a0d-9247-32ab0d61a8c7");

    [Fact]
    public async Task EstimateOrbitalTravelReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-travel/estimate", ValidRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task EstimateOrbitalTravelReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-travel/estimate", ValidRequest());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task EstimateOrbitalTravelReturnsBadRequestForInvalidRequest()
    {
        using var client = CreateConfiguredClient(SuccessfulResult());

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-travel/estimate", new { });
        var payload = await response.Content.ReadFromJsonAsync<EstimateOrbitalTravelResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(EstimateOrbitalTravelResultStatus.ValidationFailed, payload.Status);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
        Assert.Contains("Orbital group id is required.", payload.Errors);
        Assert.Contains("Destination planet id is required.", payload.Errors);
    }

    [Fact]
    public async Task EstimateOrbitalTravelReturnsBadRequestForEmptyIdentifiers()
    {
        using var client = CreateConfiguredClient(SuccessfulResult());

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-travel/estimate", new
        {
            civilizationId = Guid.Empty,
            orbitalGroupId = Guid.Empty,
            destinationPlanetId = Guid.Empty
        });
        var payload = await response.Content.ReadFromJsonAsync<EstimateOrbitalTravelResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(EstimateOrbitalTravelResultStatus.ValidationFailed, payload.Status);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
        Assert.Contains("Orbital group id is required.", payload.Errors);
        Assert.Contains("Destination planet id is required.", payload.Errors);
    }

    [Fact]
    public async Task EstimateOrbitalTravelReturnsOkForValidPreview()
    {
        using var client = CreateConfiguredClient(SuccessfulResult());

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-travel/estimate", ValidRequest());
        var payload = await response.Content.ReadFromJsonAsync<EstimateOrbitalTravelResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(EstimateOrbitalTravelResultStatus.Succeeded, payload.Status);
        Assert.True(payload.Succeeded);
        Assert.Equal(OrbitalGroupId, payload.OrbitalGroupId);
        Assert.Equal(CurrentPlanetId, payload.CurrentPlanetId);
        Assert.Equal(DestinationPlanetId, payload.DestinationPlanetId);
        Assert.Equal(1, payload.AbstractDistanceUnits);
        Assert.Equal(TimeSpan.FromHours(1), payload.EstimatedDuration);
        Assert.NotNull(payload.RouteProfile);
        Assert.Equal(OrbitalRouteClass.LocalOrbit, payload.RouteProfile.RouteClass);
        Assert.Equal(1, payload.RouteProfile.DistanceBand);
        Assert.Equal(OrbitalRouteRiskBand.Low, payload.RouteProfile.RiskBand);
        Assert.Equal(1m, payload.RouteProfile.FuelMultiplier);
        Assert.True(payload.RouteProfile.IsSupported);
        Assert.NotNull(payload.FuelReadiness);
        Assert.Equal(1m, payload.FuelReadiness.EstimatedFuelUnitsRequired);
        Assert.Equal(6, payload.FuelReadiness.EstimatedRangeUnitsAvailable);
        Assert.True(payload.FuelReadiness.IsFuelReady);
        Assert.Null(payload.FuelReadiness.NotReadyReason);
        Assert.Contains(payload.ResourceCosts, x => x.ResourceType == ResourceType.Credits && x.Quantity == 5m);
        Assert.Contains(payload.ResourceCosts, x => x.ResourceType == ResourceType.Gas && x.Quantity == 2m);
        Assert.True(payload.CanAfford);
        Assert.Empty(payload.InsufficientResources);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task EstimateOrbitalTravelReturnsNotFoundWhenServiceCannotFindEntities()
    {
        using var client = CreateConfiguredClient(
            EstimateOrbitalTravelResult.NotFound("Destination planet was not found."));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-travel/estimate", ValidRequest());
        var payload = await response.Content.ReadFromJsonAsync<EstimateOrbitalTravelResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(EstimateOrbitalTravelResultStatus.NotFound, payload.Status);
        Assert.False(payload.Succeeded);
        Assert.Contains("Destination planet was not found.", payload.Errors);
    }

    [Fact]
    public async Task EstimateOrbitalTravelReturnsConflictWhenServiceRejectsRequest()
    {
        using var client = CreateConfiguredClient(
            EstimateOrbitalTravelResult.Conflict("Orbital group already has an active transfer."));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-travel/estimate", ValidRequest());
        var payload = await response.Content.ReadFromJsonAsync<EstimateOrbitalTravelResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(EstimateOrbitalTravelResultStatus.Conflict, payload.Status);
        Assert.False(payload.Succeeded);
        Assert.Contains("Orbital group already has an active transfer.", payload.Errors);
    }

    [Fact]
    public async Task EstimateOrbitalTravelSucceedsForSeededScenarioWithoutMutatingState()
    {
        const string databaseName = "dev-orbital-travel-estimate-seeded-scenario";
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "minimal-validation" }))
        {
            var seedBody = await seedResponse.Content.ReadAsStringAsync();
            Assert.True(seedResponse.IsSuccessStatusCode, seedBody);
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
            var seedPayload = System.Text.Json.JsonSerializer.Deserialize<ApplyDevelopmentSeedResponse>(
                seedBody,
                new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
            Assert.NotNull(seedPayload);
            Assert.True(seedPayload.Succeeded, seedBody);
            Assert.Equal("minimal-validation", seedPayload.Profile);
        }

        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var seededGroup = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x =>
                x.CivilizationId == SeededCivilizationId &&
                x.CurrentPlanetId == SeededOwnedPlanetId &&
                x.Status == OrbitalGroupStatus.Stationed)
            .OrderByDescending(x => x.Quantity)
            .FirstAsync();
        var stockpileBefore = await dbContext.PlanetResourceStockpiles
            .AsNoTracking()
            .SingleAsync(x => x.PlanetId == SeededOwnedPlanetId);
        var transferCountBefore = await dbContext.Set<OrbitalTransfer>().CountAsync();

        using (var uiStateResponse = await client.GetAsync($"/api/dev/fleets/ui-state?civilizationId={SeededCivilizationId}"))
        {
            var uiStatePayload = await uiStateResponse.Content.ReadFromJsonAsync<DevFleetUiStateResponse>();
            Assert.Equal(HttpStatusCode.OK, uiStateResponse.StatusCode);
            Assert.NotNull(uiStatePayload?.UiState);
            Assert.Contains(uiStatePayload.UiState.Groups, x => x.Id == seededGroup.Id && x.Status == OrbitalGroupStatus.Stationed);
            Assert.Contains(uiStatePayload.UiState.ResourceContexts, x =>
                x.PlanetId == SeededOwnedPlanetId &&
                x.Balances.Any(balance => balance.ResourceType == ResourceType.Credits && balance.Quantity == 125));
        }

        using (var estimateResponse = await client.PostAsJsonAsync("/api/dev/fleets/orbital-travel/estimate", new
        {
            civilizationId = SeededCivilizationId,
            orbitalGroupId = seededGroup.Id,
            destinationPlanetId = SeededDestinationPlanetId
        }))
        {
            var estimatePayload = await estimateResponse.Content.ReadFromJsonAsync<EstimateOrbitalTravelResponse>();
            Assert.Equal(HttpStatusCode.OK, estimateResponse.StatusCode);
            Assert.NotNull(estimatePayload);
            Assert.Equal(EstimateOrbitalTravelResultStatus.Succeeded, estimatePayload.Status);
            Assert.True(estimatePayload.Succeeded);
            Assert.Equal(seededGroup.Id, estimatePayload.OrbitalGroupId);
            Assert.Equal(SeededOwnedPlanetId, estimatePayload.CurrentPlanetId);
            Assert.Equal(SeededDestinationPlanetId, estimatePayload.DestinationPlanetId);
            Assert.Equal(1, estimatePayload.AbstractDistanceUnits);
            Assert.Equal(TimeSpan.FromHours(1), estimatePayload.EstimatedDuration);
            Assert.NotNull(estimatePayload.RouteProfile);
            Assert.Equal(OrbitalRouteClass.LocalOrbit, estimatePayload.RouteProfile.RouteClass);
            Assert.NotNull(estimatePayload.FuelReadiness);
            Assert.True(estimatePayload.FuelReadiness.IsFuelReady);
            Assert.Contains(estimatePayload.ResourceCosts, x => x.ResourceType == ResourceType.Credits && x.Quantity == 5m);
            Assert.Contains(estimatePayload.ResourceCosts, x => x.ResourceType == ResourceType.Gas && x.Quantity == 2m);
            Assert.True(estimatePayload.CanAfford);
            Assert.Empty(estimatePayload.InsufficientResources);
        }

        var transferCountAfter = await dbContext.Set<OrbitalTransfer>().CountAsync();
        var persistedGroup = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .SingleAsync(x => x.Id == seededGroup.Id);
        var stockpileAfter = await dbContext.PlanetResourceStockpiles
            .AsNoTracking()
            .SingleAsync(x => x.PlanetId == SeededOwnedPlanetId);

        Assert.Equal(transferCountBefore, transferCountAfter);
        Assert.Equal(OrbitalGroupStatus.Stationed, persistedGroup.Status);
        Assert.Equal(stockpileBefore.Credits, stockpileAfter.Credits);
        Assert.Equal(stockpileBefore.Metal, stockpileAfter.Metal);
        Assert.Equal(stockpileBefore.Crystal, stockpileAfter.Crystal);
        Assert.Equal(stockpileBefore.Gas, stockpileAfter.Gas);
    }

    private HttpClient CreateConfiguredClient(EstimateOrbitalTravelResult result) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IOrbitalTravelEstimateService>(new FakeOrbitalTravelEstimateService(result));
            });
        }).CreateClient();

    private static EstimateOrbitalTravelResult SuccessfulResult() =>
        EstimateOrbitalTravelResult.Success(
            OrbitalGroupId,
            CurrentPlanetId,
            DestinationPlanetId,
            1,
            TimeSpan.FromHours(1),
            new OrbitalRouteProfileDto(
                OrbitalRouteClass.LocalOrbit,
                1,
                OrbitalRouteRiskBand.Low,
                1m,
                ["Single-hop local orbital transfer."],
                true),
            new OrbitalFuelReadinessDto(1m, 6, true, null, OrbitalFuelReadinessPolicy.PlaceholderDerived),
            [
                new OrbitalTravelCostComponentDto(ResourceType.Credits, 5m),
                new OrbitalTravelCostComponentDto(ResourceType.Gas, 2m)
            ],
            true,
            []);

    private static object ValidRequest() => new
    {
        civilizationId = CivilizationId,
        orbitalGroupId = OrbitalGroupId,
        destinationPlanetId = DestinationPlanetId
    };

    private sealed class FakeOrbitalTravelEstimateService(EstimateOrbitalTravelResult result)
        : IOrbitalTravelEstimateService
    {
        public Task<EstimateOrbitalTravelResult> EstimateAsync(
            EstimateOrbitalTravelRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed record EstimateOrbitalTravelResponse(
        EstimateOrbitalTravelResultStatus Status,
        bool Succeeded,
        Guid? OrbitalGroupId,
        Guid? CurrentPlanetId,
        Guid? DestinationPlanetId,
        int AbstractDistanceUnits,
        TimeSpan? EstimatedDuration,
        OrbitalRouteProfileResponse? RouteProfile,
        OrbitalFuelReadinessResponse? FuelReadiness,
        OrbitalTravelCostComponentResponse[] ResourceCosts,
        bool CanAfford,
        OrbitalTravelInsufficientResourceResponse[] InsufficientResources,
        string[] Errors);

    private sealed record OrbitalRouteProfileResponse(
        OrbitalRouteClass RouteClass,
        int DistanceBand,
        OrbitalRouteRiskBand RiskBand,
        decimal FuelMultiplier,
        string[] ComplexityNotes,
        bool IsSupported);

    private sealed record OrbitalFuelReadinessResponse(
        decimal EstimatedFuelUnitsRequired,
        int EstimatedRangeUnitsAvailable,
        bool IsFuelReady,
        string? NotReadyReason,
        OrbitalFuelReadinessPolicy Policy);

    private sealed record OrbitalTravelCostComponentResponse(ResourceType ResourceType, decimal Quantity);

    private sealed record OrbitalTravelInsufficientResourceResponse(
        ResourceType ResourceType,
        decimal RequiredQuantity,
        decimal AvailableQuantity);

    private sealed record ApplyDevelopmentSeedResponse(
        bool Succeeded,
        string? Profile,
        string[] AppliedSteps,
        string[] Errors);

    private sealed record DevFleetUiStateResponse(
        bool Succeeded,
        GetDevFleetUiStateResult? UiState,
        string[] Errors);
}
