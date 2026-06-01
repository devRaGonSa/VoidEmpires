using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.StrategicMap;

namespace VoidEmpires.Tests;

public class DevDetectionCoverageEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("48fd6134-d164-4361-b70c-c8ba308d6008");

    [Fact]
    public async Task DetectionCoverageReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();
        using var response = await client.GetAsync($"/api/dev/strategic-map/detection-coverage?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DetectionCoverageReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = string.Empty }));
        }).CreateClient();

        using var response = await client.GetAsync($"/api/dev/strategic-map/detection-coverage?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?civilizationId=00000000-0000-0000-0000-000000000000")]
    public async Task DetectionCoverageReturnsBadRequestForMissingOrEmptyCivilizationId(string queryString)
    {
        using var client = CreateConfiguredClient(new FakeDetectionCoverageService(new GetDetectionCoverageResult(CivilizationId, [])));
        using var response = await client.GetAsync($"/api/dev/strategic-map/detection-coverage{queryString}");
        var payload = await response.Content.ReadFromJsonAsync<DetectionCoverageResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.DetectionCoverage);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task DetectionCoverageReturnsOkForValidReadOnlyRequest()
    {
        var coverage = new DetectionCoverageDto(Guid.NewGuid(), DetectionCoverageSourceKind.Planet, DetectionCoverageClass.Orbital,
            Guid.NewGuid(), Guid.NewGuid(), null, 2, 100, [Guid.NewGuid()], "Metadata only.", "Read-only.");
        var fakeService = new FakeDetectionCoverageService(new GetDetectionCoverageResult(CivilizationId, [coverage]));
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.GetAsync($"/api/dev/strategic-map/detection-coverage?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<DetectionCoverageResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.DetectionCoverage);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        Assert.Equal(DetectionCoverageClass.Orbital, Assert.Single(payload.DetectionCoverage.Coverages).CoverageClass);
    }

    private HttpClient CreateConfiguredClient(IDetectionCoverageService detectionCoverageService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_detection_coverage_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(detectionCoverageService));
        }).CreateClient();

    private sealed class FakeDetectionCoverageService(GetDetectionCoverageResult result) : IDetectionCoverageService
    {
        public GetDetectionCoverageRequest? LastRequest { get; private set; }

        public Task<GetDetectionCoverageResult> GetAsync(GetDetectionCoverageRequest request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record DetectionCoverageResponse(
        bool Succeeded,
        GetDetectionCoverageResult? DetectionCoverage,
        string[] Errors);
}
