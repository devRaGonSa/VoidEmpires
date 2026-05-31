using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Tests;

public class DevOrbitalTravelEstimateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
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
        using var client = factory.CreateClient();

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
    public async Task EstimateOrbitalTravelReturnsConflictWhenServiceRejectsRequest()
    {
        using var client = CreateConfiguredClient(
            EstimateOrbitalTravelResult.Failure("Orbital group already has an active transfer."));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-travel/estimate", ValidRequest());
        var payload = await response.Content.ReadFromJsonAsync<EstimateOrbitalTravelResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Orbital group already has an active transfer.", payload.Errors);
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
}
