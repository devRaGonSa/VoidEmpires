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

public class DevOrbitalGroupLookupEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("3659791b-71b4-4582-b9cd-ee396930d075");
    private static readonly Guid OriginPlanetId = Guid.Parse("4813dab0-0de6-454c-8c6c-d70944b87bfa");
    private static readonly Guid CurrentPlanetId = Guid.Parse("0dd85c9f-7c1d-4a0d-9247-32ab0d61a8c7");
    private static readonly Guid OrbitalGroupId = Guid.Parse("4a332c10-5a76-407e-8fb5-d1a29ad568bc");

    [Fact]
    public async Task ListOrbitalGroupsReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/fleets/orbital-groups?civilizationId={CivilizationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListOrbitalGroupsReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.GetAsync($"/api/dev/fleets/orbital-groups?civilizationId={CivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task ListOrbitalGroupsReturnsBadRequestWhenCivilizationIdIsMissing()
    {
        using var client = CreateConfiguredClient([]);

        using var response = await client.GetAsync("/api/dev/fleets/orbital-groups");
        var payload = await response.Content.ReadFromJsonAsync<OrbitalGroupListResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Empty(payload.Groups);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task ListOrbitalGroupsReturnsGroupsForCivilization()
    {
        using var client = CreateConfiguredClient([
            new OrbitalGroupQueryItem(
                OrbitalGroupId,
                CivilizationId,
                OriginPlanetId,
                CurrentPlanetId,
                SpaceAssetType.ScoutCraft,
                2,
                OrbitalGroupStatus.Stationed,
                true)
        ]);

        using var response = await client.GetAsync($"/api/dev/fleets/orbital-groups?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<OrbitalGroupListResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        var group = Assert.Single(payload.Groups);
        Assert.Equal(OrbitalGroupId, group.Id);
        Assert.Equal(CivilizationId, group.CivilizationId);
        Assert.Equal(OriginPlanetId, group.OriginPlanetId);
        Assert.Equal(CurrentPlanetId, group.CurrentPlanetId);
        Assert.Equal(SpaceAssetType.ScoutCraft, group.AssetType);
        Assert.Equal(2, group.Quantity);
        Assert.Equal(OrbitalGroupStatus.Stationed, group.Status);
        Assert.True(group.IsStationedAwayFromOrigin);
    }

    [Fact]
    public async Task ListOrbitalGroupsPassesOptionalFiltersToService()
    {
        var fakeService = new FakeOrbitalGroupLookupService([]);
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.GetAsync(
            $"/api/dev/fleets/orbital-groups?civilizationId={CivilizationId}&currentPlanetId={CurrentPlanetId}&originPlanetId={OriginPlanetId}&assetType={SpaceAssetType.ScoutCraft}&status={OrbitalGroupStatus.Stationed}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(fakeService.LastRequest);
        Assert.Equal(CivilizationId, fakeService.LastRequest.CivilizationId);
        Assert.Equal(CurrentPlanetId, fakeService.LastRequest.CurrentPlanetId);
        Assert.Equal(OriginPlanetId, fakeService.LastRequest.OriginPlanetId);
        Assert.Equal(SpaceAssetType.ScoutCraft, fakeService.LastRequest.AssetType);
        Assert.Equal(OrbitalGroupStatus.Stationed, fakeService.LastRequest.Status);
    }

    private HttpClient CreateConfiguredClient(IReadOnlyList<OrbitalGroupQueryItem> groups) =>
        CreateConfiguredClient(new FakeOrbitalGroupLookupService(groups));

    private HttpClient CreateConfiguredClient(FakeOrbitalGroupLookupService fakeService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IOrbitalGroupLookupService>(fakeService);
            });
        }).CreateClient();

    private sealed class FakeOrbitalGroupLookupService(IReadOnlyList<OrbitalGroupQueryItem> groups) : IOrbitalGroupLookupService
    {
        public OrbitalGroupQueryRequest? LastRequest { get; private set; }

        public Task<IReadOnlyList<OrbitalGroupQueryItem>> ListAsync(
            OrbitalGroupQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(groups);
        }
    }

    private sealed record OrbitalGroupListItemResponse(
        Guid Id,
        Guid CivilizationId,
        Guid OriginPlanetId,
        Guid CurrentPlanetId,
        SpaceAssetType AssetType,
        int Quantity,
        OrbitalGroupStatus Status,
        bool IsStationedAwayFromOrigin);

    private sealed record OrbitalGroupListResponse(
        bool Succeeded,
        OrbitalGroupListItemResponse[] Groups,
        string[] Errors);
}
