using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Assets;
using VoidEmpires.Application.Development;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Population;
using VoidEmpires.Infrastructure.Assets;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevShipyardUiStateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task ShipyardUiStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ShipyardUiStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task ShipyardUiStateReturnsBadRequestWhenCivilizationIdIsMissing()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync("/api/dev/shipyard/ui-state");
        var payload = await response.Content.ReadFromJsonAsync<DevShipyardUiStateResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task ShipyardUiStateReturnsSeededUsableCatalogWithoutMutatingState()
    {
        await using var dbContext = CreateSeededDbContext();
        var initialOrderCount = await dbContext.Set<VoidEmpires.Domain.Assets.AssetProductionOrder>().CountAsync();
        var initialStockRows = await dbContext.Set<VoidEmpires.Domain.Assets.OrbitalAssetStock>().CountAsync();
        var initialGroupCount = await dbContext.Set<VoidEmpires.Domain.Fleets.OrbitalGroup>().CountAsync();

        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<DevShipyardUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Shipyard);
        Assert.True(payload.Succeeded);
        Assert.Equal(SeedOwnedPlanetId, payload.UiState.SelectedPlanetId);
        Assert.Equal("Aurelia", payload.UiState.Shipyard.PlanetName);
        Assert.Empty(payload.UiState.Shipyard.Queue);
        Assert.NotEmpty(payload.UiState.Shipyard.ResourceStockpile);
        Assert.Contains(payload.UiState.Shipyard.OrbitalStock, x => x.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.EscortCraft && x.Quantity == 4);
        Assert.Equal(4, payload.UiState.Shipyard.Catalog.Count);
        Assert.Equal(1, payload.UiState.Shipyard.Catalog.Count(item => item.AvailabilityStatus == "Available"));
        Assert.Equal(3, payload.UiState.Shipyard.Catalog.Count(item => item.AvailabilityStatus == "Blocked"));
        Assert.Contains(payload.UiState.Shipyard.Catalog, item => item.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.ScoutCraft && item.AvailabilityStatus == "Available");
        Assert.Contains(payload.UiState.Shipyard.Catalog, item => item.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.CargoCraft && item.AvailabilityReason == "InsufficientResources");
        Assert.Contains(payload.UiState.Shipyard.Catalog, item => item.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.EscortCraft && item.AvailabilityReason == "MissingRequiredBuilding");
        Assert.True(payload.UiState.Shipyard.ActionSummary.EnqueueSupported);
        Assert.Equal("Available", payload.UiState.Shipyard.ActionSummary.EnqueueActionStatus);
        Assert.Equal(initialOrderCount, await dbContext.Set<VoidEmpires.Domain.Assets.AssetProductionOrder>().CountAsync());
        Assert.Equal(initialStockRows, await dbContext.Set<VoidEmpires.Domain.Assets.OrbitalAssetStock>().CountAsync());
        Assert.Equal(initialGroupCount, await dbContext.Set<VoidEmpires.Domain.Fleets.OrbitalGroup>().CountAsync());
    }

    [Fact]
    public async Task ShipyardUiStateReturnsAvailableAndBlockedCatalogOptionsWhenLocalPrerequisitesExist()
    {
        await using var dbContext = CreateSeededDbContext(context =>
        {
            var stockpile = context.PlanetResourceStockpiles.Single(x => x.PlanetId == SeedOwnedPlanetId);
            stockpile.Increase(ResourceType.Metal, 300);
            stockpile.Increase(ResourceType.Crystal, 200);
            stockpile.Increase(ResourceType.Gas, 100);
        });
        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<DevShipyardUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Shipyard);
        Assert.Contains(payload.UiState.Shipyard.Catalog, x => x.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.ScoutCraft && x.AvailabilityStatus == "Available");
        Assert.Contains(payload.UiState.Shipyard.Catalog, x => x.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.CargoCraft && x.AvailabilityStatus == "Available");
        Assert.Contains(payload.UiState.Shipyard.Catalog, x => x.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.EscortCraft && x.AvailabilityReason == "MissingRequiredBuilding");
        Assert.True(payload.UiState.Shipyard.ActionSummary.EnqueueSupported);
        Assert.Equal("Available", payload.UiState.Shipyard.ActionSummary.EnqueueActionStatus);
    }

    [Fact]
    public async Task ShipyardUiStateReturnsNotFoundForUnknownPlanet()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());

        using var response = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<DevShipyardUiStateResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Planet was not found.", payload.Errors);
    }

    [Fact]
    public async Task ShipyardValidationProfileReturnsRicherAvailableBlockedStockAndQueueState()
    {
        await using var dbContext = CreateSeededDbContext(profile: "shipyard-validation");
        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/shipyard/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevShipyardUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.Shipyard);
        Assert.Single(payload.UiState.Shipyard.Queue);
        Assert.Equal(2, payload.UiState.Shipyard.OrbitalStock.Count);
        Assert.Contains(payload.UiState.Shipyard.OrbitalStock, x => x.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.ScoutCraft && x.Quantity == 1);
        Assert.Contains(payload.UiState.Shipyard.OrbitalStock, x => x.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.EscortCraft && x.Quantity == 4);
        Assert.Equal(1, payload.UiState.Shipyard.Catalog.Count(item => item.AvailabilityStatus == "Available"));
        Assert.Equal(3, payload.UiState.Shipyard.Catalog.Count(item => item.AvailabilityStatus == "Blocked"));
        Assert.Contains(payload.UiState.Shipyard.Catalog, item => item.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.ScoutCraft && item.AvailabilityStatus == "Available");
        Assert.Contains(payload.UiState.Shipyard.Catalog, item => item.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.CargoCraft && item.AvailabilityReason == "InsufficientResources");
        Assert.Equal(
            2,
            payload.UiState.Shipyard.Catalog
                .Where(item => item.AvailabilityStatus == "Blocked")
                .Select(item => item.AvailabilityReason)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.True(payload.UiState.Shipyard.ActionSummary.EnqueueSupported);
        Assert.Equal("Available", payload.UiState.Shipyard.ActionSummary.EnqueueActionStatus);
    }

    [Fact]
    public async Task ReapplyingShipyardValidationProfileDoesNotDuplicateCompletedOrdersOrStockRows()
    {
        await using var dbContext = CreateSeededDbContext(profile: "shipyard-validation");
        var service = new DevelopmentSeedService(dbContext);

        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("shipyard-validation"));

        Assert.Equal(
            1,
            await dbContext.Set<VoidEmpires.Domain.Assets.AssetProductionOrder>().CountAsync(x =>
                x.PlanetId == SeedOwnedPlanetId &&
                x.Target == VoidEmpires.Domain.Assets.AssetProductionTarget.Orbital &&
                x.SpaceAssetType == VoidEmpires.Domain.Assets.SpaceAssetType.ScoutCraft &&
                x.Status == VoidEmpires.Domain.Assets.AssetProductionOrderStatus.Completed));
        Assert.Equal(
            1,
            await dbContext.Set<VoidEmpires.Domain.Assets.OrbitalAssetStock>().CountAsync(x =>
                x.PlanetId == SeedOwnedPlanetId &&
                x.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.ScoutCraft));
        Assert.Equal(
            1,
            await dbContext.Set<VoidEmpires.Domain.Assets.OrbitalAssetStock>().CountAsync(x =>
                x.PlanetId == SeedOwnedPlanetId &&
                x.AssetType == VoidEmpires.Domain.Assets.SpaceAssetType.EscortCraft));
    }

    private HttpClient CreateConfiguredClient(VoidEmpiresDbContext dbContext) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_shipyard_ui_state_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IDevShipyardUiStateService>(new DevShipyardUiStateService(dbContext));
            });
        }).CreateClient();

    private static VoidEmpiresDbContext CreateSeededDbContext(Action<VoidEmpiresDbContext>? seedOverride = null, string profile = "minimal-validation")
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest(profile)).GetAwaiter().GetResult();
        seedOverride?.Invoke(dbContext);
        dbContext.SaveChanges();

        return dbContext;
    }

    private sealed record DevShipyardUiStateResponse(
        bool Succeeded,
        GetDevShipyardUiStateResult? UiState,
        string[] Errors);
}
