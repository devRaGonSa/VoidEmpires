using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class DevStrategicMapEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedSystemId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedVisibleComparisonPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");
    private static readonly Guid SeedKnownComparisonPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000003");
    private static readonly Guid CivilizationId = Guid.Parse("5d6b762b-fdd9-453f-b6b4-67d36c7e2cb4");
    private static readonly Guid OtherCivilizationId = Guid.Parse("4921e837-a3e7-4b65-b7a3-5d3086851ed0");

    [Fact]
    public async Task StrategicMapReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/strategic-map?civilizationId={CivilizationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task StrategicMapReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = string.Empty
                }));
        }).CreateClient();

        using var response = await client.GetAsync($"/api/dev/strategic-map?civilizationId={CivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?civilizationId=00000000-0000-0000-0000-000000000000")]
    public async Task StrategicMapReturnsBadRequestForMissingOrEmptyCivilizationId(string queryString)
    {
        using var client = CreateConfiguredClient(new GetStrategicMapResult(CivilizationId, [], [], [], [], []));

        using var response = await client.GetAsync($"/api/dev/strategic-map{queryString}");
        var payload = await response.Content.ReadFromJsonAsync<StrategicMapResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.Map);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task StrategicMapReturnsOkForValidRequest()
    {
        var fakeService = new FakeStrategicMapService(CreateMapResult());
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.GetAsync($"/api/dev/strategic-map?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<StrategicMapResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.NotNull(payload.Map);
        Assert.Equal(CivilizationId, payload.Map.CivilizationId);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        var system = Assert.Single(payload.Map.Systems);
        Assert.Equal(MapVisibilityLevel.Visible, system.VisibilityLevel);
        Assert.True(system.IsVisible);
        Assert.False(system.ExplorationPreview.CanPreviewExploration);
        Assert.Contains(system.Commands, x => x.ActionKey == "strategicMap.system.view" && x.IsAvailable);
        Assert.Contains(system.Commands, x => x.ActionKey == "exploration.preview" && !x.IsAvailable);
        Assert.Contains(system.Planets, x => x.VisibilityLevel == MapVisibilityLevel.Owned && x.IsVisible);
    }

    [Fact]
    public async Task StrategicMapResponseKeepsOtherCivilizationOwnershipExcluded()
    {
        using var client = CreateConfiguredClient(CreateMapResult());

        using var response = await client.GetAsync($"/api/dev/strategic-map?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<StrategicMapResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.Map);
        var planets = payload.Map.Systems.Single().Planets;
        Assert.Contains(planets, x => x.IsOwnedByRequestingCivilization && x.CivilizationId == CivilizationId);
        Assert.DoesNotContain(planets, x => x.CivilizationId == OtherCivilizationId);
    }

    [Fact]
    public async Task StrategicMapEndpointReturnsNonEmptyCockpitValidationSeededReadModel()
    {
        await using var dbContext = CreateSeededDbContext("cockpit-validation");
        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/strategic-map?civilizationId={SeedCivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<StrategicMapResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.Map);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.Equal(SeedCivilizationId, payload.Map.CivilizationId);
        Assert.NotEmpty(payload.Map.Systems);

        var system = Assert.Single(payload.Map.Systems);
        Assert.Equal(SeedSystemId, system.SystemId);
        Assert.Equal("Helios Gate", system.SystemName);
        Assert.True(system.IsVisible);
        Assert.True(system.Planets.Count >= 3);
        Assert.Contains(system.Planets, x => x.PlanetId == SeedOwnedPlanetId && x.PlanetName == "Aurelia" && x.IsOwnedByRequestingCivilization);
        Assert.Contains(system.Planets, x => x.PlanetId == SeedVisibleComparisonPlanetId && x.PlanetName == "Cinder Reach" && x.IsVisible && !x.IsOwnedByRequestingCivilization);
        Assert.Contains(system.Planets, x => x.PlanetId == SeedKnownComparisonPlanetId && x.PlanetName == "Aether Crown" && x.IsVisible && !x.IsOwnedByRequestingCivilization);
        Assert.Equal(4, system.FleetPresence.Count);
        Assert.Single(system.TransferOverlays);
    }

    [Fact]
    public async Task GalaxyCockpitRegressionSmoke_StrategicMapEndpointKeepsNonEmptyProjectableFocusablePayload()
    {
        await using var dbContext = CreateSeededDbContext("cockpit-validation");
        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/strategic-map?civilizationId={SeedCivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<StrategicMapResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.Map);
        Assert.True(payload.Succeeded);
        Assert.True(payload.Map.Systems.Count >= 1);
        Assert.Contains(payload.Map.Systems, system => system.IsVisible || system.IsOwnedByRequestingCivilization);

        var focusableSystem = payload.Map.Systems
            .FirstOrDefault(system => system.IsOwnedByRequestingCivilization)
            ?? payload.Map.Systems.FirstOrDefault(system => system.IsVisible);

        Assert.NotNull(focusableSystem);
        Assert.NotEqual(Guid.Empty, focusableSystem.SystemId);
        Assert.False(string.IsNullOrWhiteSpace(focusableSystem.SystemName));
        Assert.Contains(payload.Map.Systems, system =>
            system.CoordinateX == 12 &&
            system.CoordinateY == -4 &&
            system.CoordinateZ == 3);
        Assert.True(payload.Map.Systems.Sum(system => system.Planets.Count) >= 1);
        Assert.True(
            focusableSystem.FleetPresence.Count > 0 ||
            focusableSystem.TransferOverlays.Count > 0);
    }

    private HttpClient CreateConfiguredClient(GetStrategicMapResult result) =>
        CreateConfiguredClient(new FakeStrategicMapService(result));

    private HttpClient CreateConfiguredClient(IStrategicMapService strategicMapService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_strategic_map_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(strategicMapService));
        }).CreateClient();

    private HttpClient CreateConfiguredClient(VoidEmpiresDbContext dbContext) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_strategic_map_seeded_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IStrategicMapService>(new StrategicMapService(
                    dbContext,
                    new SystemVisualStateService(dbContext, new PlanetVisualStateService(dbContext)),
                    new MapVisibilityService(dbContext)));
            });
        }).CreateClient();

    private static VoidEmpiresDbContext CreateSeededDbContext(string profile)
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest(profile)).GetAwaiter().GetResult();
        return dbContext;
    }

    private static GetStrategicMapResult CreateMapResult()
    {
        var ownedPlanetId = Guid.Parse("5880c6bf-74af-4ab7-a7a4-e4cc4116de43");
        var otherPlanetId = Guid.Parse("f509c22f-1178-407a-9421-563449e4455c");

        return new GetStrategicMapResult(
            CivilizationId,
            [
                new StrategicMapSystemDto(
                    Guid.Parse("6425dd52-2317-4670-bec8-37805af37fbb"),
                    Guid.Parse("4a4b9934-3932-4451-a590-b924c92f31be"),
                    "Helios",
                    1,
                    2,
                    3,
                    StarType.YellowDwarf,
                    MapVisibilityLevel.Visible,
                    MapVisibilityReason.SystemContainsOwnedPlanet,
                    true,
                    true,
                    new StrategicMapExplorationPreviewDto(false, ExplorationActionBlockReason.AlreadyVisible, "Already visible."),
                    [
                        new StrategicMapCommandAvailabilityDto("strategicMap.system.view", true, StrategicMapCommandBlockReason.None, "Visible."),
                        new StrategicMapCommandAvailabilityDto("exploration.preview", false, StrategicMapCommandBlockReason.ExplorationPreviewUnavailable, "Already visible.")
                    ],
                    [
                        new StrategicMapPlanetDto(
                            ownedPlanetId,
                            "Owned",
                            PlanetType.Terran,
                            100,
                            PlanetColonizationStatus.Colonized,
                            true,
                            MapVisibilityLevel.Owned,
                            MapVisibilityReason.OwnedPlanet,
                            true,
                            new StrategicMapExplorationPreviewDto(false, ExplorationActionBlockReason.AlreadyOwned, "Already owned."),
                            [new StrategicMapCommandAvailabilityDto("strategicMap.planet.viewDetail", true, StrategicMapCommandBlockReason.None, "Visible.")],
                            CivilizationId,
                            1,
                            5.75f,
                            47f,
                            1f,
                            0.3f,
                            0.1f,
                            0.2f,
                            0f,
                            0f),
                        new StrategicMapPlanetDto(
                            otherPlanetId,
                            "Known",
                            PlanetType.Desert,
                            90,
                            PlanetColonizationStatus.Colonized,
                            false,
                            MapVisibilityLevel.Visible,
                            MapVisibilityReason.SystemContainsOwnedPlanet,
                            true,
                            new StrategicMapExplorationPreviewDto(false, ExplorationActionBlockReason.AlreadyVisible, "Already visible."),
                            [new StrategicMapCommandAvailabilityDto("strategicMap.planet.viewDetail", true, StrategicMapCommandBlockReason.None, "Visible.")],
                            null,
                            2,
                            7.5f,
                            94f,
                            0.9f,
                            0.2f,
                            0f,
                            0f,
                            0f,
                            0f)
                    ],
                    [],
                    [])
            ],
            [new StrategicMapRouteFuelNoteDto("fleet.travel.estimate", true, OrbitalFuelReadinessPolicy.PlaceholderDerived, "Requires destinationPlanetId.")],
            [],
            [],
            []);
    }

    private sealed class FakeStrategicMapService(GetStrategicMapResult result) : IStrategicMapService
    {
        public GetStrategicMapRequest? LastRequest { get; private set; }

        public Task<GetStrategicMapResult> GetAsync(
            GetStrategicMapRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record StrategicMapResponse(
        bool Succeeded,
        GetStrategicMapResult? Map,
        string[] Errors);
}
