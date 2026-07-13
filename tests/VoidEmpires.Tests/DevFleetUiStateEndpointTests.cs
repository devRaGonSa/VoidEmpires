using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Fleets;

namespace VoidEmpires.Tests;

public class DevFleetUiStateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("bdff59bf-3847-4cda-b742-b4bb77ebd3c4");
    private static readonly Guid PlanetId = Guid.Parse("157c2ef4-25b4-48f7-a746-e355fcf451af");

    [Fact]
    public async Task UiStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/fleets/ui-state?civilizationId={CivilizationId}&planetId={PlanetId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UiStateReturnsSuccessfulReadPayload()
    {
        var fakeService = new FakeDevFleetUiStateService(new GetDevFleetUiStateResult(CivilizationId, [], [], [], []));
        var completionService = new FakeOrbitalTransferCompletionService();
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_ui_state_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IDevFleetUiStateService>(fakeService);
                services.AddSingleton<IOrbitalTransferCompletionService>(completionService);
            });
        }).CreateClient();

        using var response = await client.GetAsync($"/api/dev/fleets/ui-state?civilizationId={CivilizationId}&planetId={PlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevFleetUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.NotNull(payload.UiState);
        Assert.Equal(CivilizationId, payload.UiState.CivilizationId);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        Assert.Equal(PlanetId, fakeService.LastRequest?.PlanetId);
        Assert.Equal(1, completionService.CallCount);
    }

    private sealed class FakeOrbitalTransferCompletionService : IOrbitalTransferCompletionService
    {
        public int CallCount { get; private set; }

        public Task<CompleteOrbitalTransfersResult> CompleteDueAsync(DateTime nowUtc, CancellationToken cancellationToken = default)
        {
            CallCount++;
            Assert.Equal(DateTimeKind.Utc, nowUtc.Kind);
            return Task.FromResult(new CompleteOrbitalTransfersResult(0, [], []));
        }
    }

    private sealed class FakeDevFleetUiStateService(GetDevFleetUiStateResult result) : IDevFleetUiStateService
    {
        public GetDevFleetUiStateRequest? LastRequest { get; private set; }

        public Task<GetDevFleetUiStateResult> GetAsync(
            GetDevFleetUiStateRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record DevFleetUiStateResponse(
        bool Succeeded,
        GetDevFleetUiStateResult? UiState,
        string[] Errors);
}
