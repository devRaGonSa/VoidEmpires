using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.StrategicMap;

namespace VoidEmpires.Tests;

public class DevExplorationActionPreviewEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("2f8a0530-1134-43b5-b4a7-d79f90f603ea");

    [Fact]
    public async Task ExplorationPreviewReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/strategic-map/exploration-preview?civilizationId={CivilizationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ExplorationPreviewReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
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

        using var response = await client.GetAsync($"/api/dev/strategic-map/exploration-preview?civilizationId={CivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?civilizationId=00000000-0000-0000-0000-000000000000")]
    public async Task ExplorationPreviewReturnsBadRequestForMissingOrEmptyCivilizationId(string queryString)
    {
        using var client = CreateConfiguredClient(new FakeExplorationActionPreviewService(new GetExplorationActionPreviewResult(CivilizationId, [], [])));

        using var response = await client.GetAsync($"/api/dev/strategic-map/exploration-preview{queryString}");
        var payload = await response.Content.ReadFromJsonAsync<ExplorationActionPreviewResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.Preview);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task ExplorationPreviewReturnsOkForValidRequest()
    {
        var fakeService = new FakeExplorationActionPreviewService(CreatePreviewResult());
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.GetAsync($"/api/dev/strategic-map/exploration-preview?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<ExplorationActionPreviewResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.NotNull(payload.Preview);
        Assert.Equal(CivilizationId, payload.Preview.CivilizationId);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        var system = Assert.Single(payload.Preview.Systems);
        Assert.True(system.CanPreviewSystemExploration);
        Assert.Contains(payload.Preview.Notes, x => x.ActionKey == "exploration.preview" && x.IsReadOnly);
    }

    private HttpClient CreateConfiguredClient(IExplorationActionPreviewService previewService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_exploration_preview_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(previewService));
        }).CreateClient();

    private static GetExplorationActionPreviewResult CreatePreviewResult()
    {
        var planetId = Guid.Parse("6d8e3235-773d-4805-8775-40885e9687e2");

        return new GetExplorationActionPreviewResult(
            CivilizationId,
            [
                new ExplorationSystemActionPreviewDto(
                    Guid.Parse("c26693f3-e28f-47b3-a517-dc7a947498b8"),
                    MapVisibilityLevel.Unknown,
                    true,
                    ExplorationActionBlockReason.None,
                    "Preview available.",
                    [new ExplorationPlanetActionPreviewDto(planetId, MapVisibilityLevel.Unknown, true, ExplorationActionBlockReason.None, "Preview available.")])
            ],
            [new ExplorationActionNoteDto("exploration.preview", true, "Read-only preview.")]);
    }

    private sealed class FakeExplorationActionPreviewService(GetExplorationActionPreviewResult result) : IExplorationActionPreviewService
    {
        public GetExplorationActionPreviewRequest? LastRequest { get; private set; }

        public Task<GetExplorationActionPreviewResult> GetAsync(
            GetExplorationActionPreviewRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record ExplorationActionPreviewResponse(
        bool Succeeded,
        GetExplorationActionPreviewResult? Preview,
        string[] Errors);
}
