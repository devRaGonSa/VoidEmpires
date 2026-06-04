using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Planets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Planets;

namespace VoidEmpires.Tests;

public class DevGroundArmyUiStateEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOuterPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");

    [Fact] public async Task GroundArmyUiStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact] public async Task GroundArmyUiStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact] public async Task GroundArmyUiStateReturnsBadRequestWhenCivilizationIdIsMissing()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());
        using var response = await client.GetAsync("/api/dev/ground-army/ui-state");
        var payload = await response.Content.ReadFromJsonAsync<DevGroundArmyUiStateResponse>();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact] public async Task GroundArmyUiStateReturnsNotFoundForUnknownPlanet()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}&planetId={Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<DevGroundArmyUiStateResponse>();
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Contains("Planet was not found.", payload.Errors);
    }

    [Fact] public async Task GroundArmyUiStateReturnsCockpitValidationReadOnlyAggregateWithoutMutatingState()
    {
        await using var dbContext = CreateSeededDbContext(profile: "cockpit-validation");
        var initialQueueCount = await dbContext.Set<AssetProductionOrder>().CountAsync();
        var initialStockCount = await dbContext.Set<PlanetaryAssetStock>().CountAsync();
        using var client = CreateConfiguredClient(dbContext);
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevGroundArmyUiStateResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.GroundArmy);
        Assert.Equal("Aurelia", payload.UiState.GroundArmy.PlanetName);
        Assert.True(payload.UiState.GroundArmy.IsOwnedByRequestingCivilization);
        Assert.NotEmpty(payload.UiState.GroundArmy.ResourceStockpile);
        Assert.NotNull(payload.UiState.GroundArmy.Population);
        Assert.Equal(4, payload.UiState.GroundArmy.Catalog.Count);
        Assert.All(payload.UiState.GroundArmy.Catalog, x => Assert.Equal("Blocked", x.AvailabilityStatus));
        Assert.Equal(initialQueueCount, await dbContext.Set<AssetProductionOrder>().CountAsync());
        Assert.Equal(initialStockCount, await dbContext.Set<PlanetaryAssetStock>().CountAsync());
    }

    [Fact] public async Task GroundArmyUiStateReturnsAvailableAndBlockedOptionsWhenLocalPrerequisitesExist()
    {
        await using var dbContext = CreateSeededDbContext(context =>
        {
            context.Set<PlanetBuilding>().Add(PlanetBuilding.Create(SeedOwnedPlanetId, BuildingType.Barracks, 1, 1));
            var stockpile = context.PlanetResourceStockpiles.Single(x => x.PlanetId == SeedOwnedPlanetId);
            stockpile.Increase(ResourceType.Credits, 300);
            stockpile.Increase(ResourceType.Metal, 300);
            stockpile.Increase(ResourceType.Crystal, 200);
            stockpile.Increase(ResourceType.Gas, 100);
        }, "cockpit-validation");
        using var client = CreateConfiguredClient(dbContext);
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevGroundArmyUiStateResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.GroundArmy);
        Assert.Contains(payload.UiState.GroundArmy.Catalog, x => x.AssetType == PlanetaryAssetType.PatrolGroup.ToString() && x.AvailabilityStatus == "Available");
        Assert.Contains(payload.UiState.GroundArmy.Catalog, x => x.AvailabilityStatus == "Blocked");
        Assert.True(payload.UiState.GroundArmy.ActionSummary.EnqueueSupported);
    }

    [Fact] public async Task GroundArmyUiStateAllowsForeignPlanetSelectionWhileKeepingManagementDataHidden()
    {
        using var client = CreateConfiguredClient(CreateSeededDbContext());
        using var response = await client.GetAsync($"/api/dev/ground-army/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOuterPlanetId}");
        var payload = await response.Content.ReadFromJsonAsync<DevGroundArmyUiStateResponse>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState?.GroundArmy);
        Assert.False(payload.UiState.GroundArmy.IsOwnedByRequestingCivilization);
        Assert.Empty(payload.UiState.GroundArmy.Catalog);
        Assert.Empty(payload.UiState.GroundArmy.Garrison);
    }

    private HttpClient CreateConfiguredClient(VoidEmpiresDbContext dbContext) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) => configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_ground_army_ui_state_tests" }));
            builder.ConfigureTestServices(services =>
            {
                var planetUiStateService = new DevPlanetUiStateService(dbContext);
                services.AddSingleton<IDevPlanetUiStateService>(planetUiStateService);
                services.AddSingleton<IDevGroundArmyUiStateService>(new DevGroundArmyUiStateService(planetUiStateService, dbContext));
            });
        }).CreateClient();

    private static VoidEmpiresDbContext CreateSeededDbContext(Action<VoidEmpiresDbContext>? seedOverride = null, string profile = "minimal-validation")
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest(profile)).GetAwaiter().GetResult();
        seedOverride?.Invoke(dbContext);
        dbContext.SaveChanges();
        return dbContext;
    }

    private sealed record DevGroundArmyUiStateResponse(bool Succeeded, GetDevGroundArmyUiStateResult? UiState, string[] Errors);
}
