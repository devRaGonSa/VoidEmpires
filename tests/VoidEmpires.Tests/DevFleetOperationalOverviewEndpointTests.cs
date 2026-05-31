using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Tests;

public class DevFleetOperationalOverviewEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("3659791b-71b4-4582-b9cd-ee396930d075");
    private static readonly Guid OrbitalGroupId = Guid.Parse("4a332c10-5a76-407e-8fb5-d1a29ad568bc");
    private static readonly Guid OriginPlanetId = Guid.Parse("4813dab0-0de6-454c-8c6c-d70944b87bfa");
    private static readonly Guid CurrentPlanetId = Guid.Parse("0dd85c9f-7c1d-4a0d-9247-32ab0d61a8c7");

    [Fact]
    public async Task OverviewReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/fleets/overview?civilizationId={CivilizationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task OverviewReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"/api/dev/fleets/overview?civilizationId={CivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task OverviewReturnsBadRequestWhenCivilizationIdIsMissing()
    {
        using var client = CreateConfiguredClient(new FakeFleetOperationalOverviewService(EmptyResult()));

        using var response = await client.GetAsync("/api/dev/fleets/overview");
        var payload = await response.Content.ReadFromJsonAsync<FleetOperationalOverviewResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.Overview);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task OverviewReturnsGroupsForCivilization()
    {
        var fakeService = new FakeFleetOperationalOverviewService(new GetFleetOperationalOverviewResult(
            CivilizationId,
            [
                new FleetOperationalGroupDto(
                    OrbitalGroupId,
                    CivilizationId,
                    OriginPlanetId,
                    CurrentPlanetId,
                    SpaceAssetType.ScoutCraft,
                    2,
                    OrbitalGroupStatus.Stationed,
                    true,
                    false,
                    null,
                    new FleetOperationalCommandAvailabilityDto(true, true, false, false))
            ]));
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.GetAsync($"/api/dev/fleets/overview?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<FleetOperationalOverviewResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.NotNull(payload.Overview);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        var group = Assert.Single(payload.Overview.Groups);
        Assert.Equal(OrbitalGroupId, group.Id);
        Assert.Equal(SpaceAssetType.ScoutCraft, group.AssetType);
        Assert.True(group.Commands.CanCreateTransfer);
        Assert.True(group.Commands.CanSplit);
        Assert.False(group.Commands.CanMerge);
        Assert.False(group.Commands.CanCancelTransfer);
    }

    private HttpClient CreateConfiguredClient(IFleetOperationalOverviewService service) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(service);
            });
        }).CreateClient();

    private static GetFleetOperationalOverviewResult EmptyResult() => new(CivilizationId, []);

    private sealed class FakeFleetOperationalOverviewService(GetFleetOperationalOverviewResult result) : IFleetOperationalOverviewService
    {
        public GetFleetOperationalOverviewRequest? LastRequest { get; private set; }

        public Task<GetFleetOperationalOverviewResult> GetAsync(
            GetFleetOperationalOverviewRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record FleetOperationalOverviewResponse(
        bool Succeeded,
        FleetOperationalOverviewPayloadResponse? Overview,
        string[] Errors);

    private sealed record FleetOperationalOverviewPayloadResponse(
        Guid CivilizationId,
        FleetOperationalGroupResponse[] Groups);

    private sealed record FleetOperationalGroupResponse(
        Guid Id,
        SpaceAssetType AssetType,
        FleetOperationalCommandAvailabilityResponse Commands);

    private sealed record FleetOperationalCommandAvailabilityResponse(
        bool CanCreateTransfer,
        bool CanSplit,
        bool CanMerge,
        bool CanCancelTransfer);
}
