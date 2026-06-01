using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Tests;

public class DevSensorProfileEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("a36f34bf-1192-4559-a6a7-24490e0cb831");

    [Fact]
    public async Task SensorProfilesReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();
        using var response = await client.GetAsync($"/api/dev/strategic-map/sensor-profiles?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SensorProfilesReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = string.Empty }));
        }).CreateClient();

        using var response = await client.GetAsync($"/api/dev/strategic-map/sensor-profiles?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?civilizationId=00000000-0000-0000-0000-000000000000")]
    public async Task SensorProfilesReturnsBadRequestForMissingOrEmptyCivilizationId(string queryString)
    {
        using var client = CreateConfiguredClient(new FakeSensorProfileService(new GetSensorProfilesResult(CivilizationId, [])));
        using var response = await client.GetAsync($"/api/dev/strategic-map/sensor-profiles{queryString}");
        var payload = await response.Content.ReadFromJsonAsync<SensorProfilesResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.SensorProfiles);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task SensorProfilesReturnsOkForValidReadOnlyRequest()
    {
        var profile = new SensorProfileDto(Guid.NewGuid(), SensorProfileSourceKind.Planet, SensorProfileClass.Orbital, Guid.NewGuid(), Guid.NewGuid(), null, null, 2, 20, "Metadata only.");
        var fakeService = new FakeSensorProfileService(new GetSensorProfilesResult(CivilizationId, [profile]));
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.GetAsync($"/api/dev/strategic-map/sensor-profiles?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<SensorProfilesResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.SensorProfiles);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        Assert.Equal(SensorProfileClass.Orbital, Assert.Single(payload.SensorProfiles.Profiles).SensorClass);
    }

    private HttpClient CreateConfiguredClient(ISensorProfileService sensorProfileService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_sensor_profile_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(sensorProfileService));
        }).CreateClient();

    private sealed class FakeSensorProfileService(GetSensorProfilesResult result) : ISensorProfileService
    {
        public GetSensorProfilesRequest? LastRequest { get; private set; }

        public Task<GetSensorProfilesResult> GetAsync(GetSensorProfilesRequest request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record SensorProfilesResponse(
        bool Succeeded,
        GetSensorProfilesResult? SensorProfiles,
        string[] Errors);
}
