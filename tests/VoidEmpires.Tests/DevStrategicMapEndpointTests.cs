using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Tests;

public class DevStrategicMapEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
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
        using var client = CreateConfiguredClient(new GetStrategicMapResult(CivilizationId, [], []));

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
        Assert.Single(payload.Map.Systems);
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
                    [
                        new StrategicMapPlanetDto(ownedPlanetId, "Owned", PlanetType.Terran, 100, PlanetColonizationStatus.Colonized, true, CivilizationId, 1, 5.75f, 47f, 1f, 0.3f, 0.1f, 0.2f, 0f, 0f),
                        new StrategicMapPlanetDto(otherPlanetId, "Known", PlanetType.Desert, 90, PlanetColonizationStatus.Colonized, false, null, 2, 7.5f, 94f, 0.9f, 0.2f, 0f, 0f, 0f, 0f)
                    ],
                    [],
                    [])
            ],
            [new StrategicMapRouteFuelNoteDto("fleet.travel.estimate", true, OrbitalFuelReadinessPolicy.PlaceholderDerived, "Requires destinationPlanetId.")]);
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
