using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;

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

    [Fact]
    public async Task ScopedFleetCreationMapsSelectedPlanetAndQuantityToAuthoritativeService()
    {
        var groupId = Guid.NewGuid();
        var groupService = new FakeOrbitalGroupService(CreateOrbitalGroupResult.Success(groupId));
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) => configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=fleet_create_endpoint_tests" }));
            builder.ConfigureTestServices(services => services.AddSingleton<IOrbitalGroupService>(groupService));
        }).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/create-from-local-stock", new
        {
            civilizationId = CivilizationId,
            planetId = PlanetId,
            assetType = SpaceAssetType.EscortCraft,
            quantity = 3
        });
        var payload = await response.Content.ReadFromJsonAsync<CreateLocalFleetResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(groupId, payload.OrbitalGroupId);
        Assert.Equal(new CreateOrbitalGroupRequest(CivilizationId, PlanetId, PlanetId, SpaceAssetType.EscortCraft, 3), groupService.LastRequest);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ScopedFleetCreationRejectsNonPositiveQuantity(int quantity)
    {
        var groupService = new FakeOrbitalGroupService(CreateOrbitalGroupResult.Success(Guid.NewGuid()));
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) => configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=fleet_create_validation_tests" }));
            builder.ConfigureTestServices(services => services.AddSingleton<IOrbitalGroupService>(groupService));
        }).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/create-from-local-stock", new
        {
            civilizationId = CivilizationId,
            planetId = PlanetId,
            assetType = SpaceAssetType.ScoutCraft,
            quantity
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Null(groupService.LastRequest);
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

    private sealed class FakeOrbitalGroupService(CreateOrbitalGroupResult result) : IOrbitalGroupService
    {
        public CreateOrbitalGroupRequest? LastRequest { get; private set; }

        public Task<CreateOrbitalGroupResult> CreateFromLocalStockAsync(CreateOrbitalGroupRequest request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
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

    private sealed record CreateLocalFleetResponse(bool Succeeded, Guid? OrbitalGroupId, string[] Errors);
}
