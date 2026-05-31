using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class StrategicMapReadinessSmokeTests
{
    [Fact]
    public async Task CurrentStrategicMapReadinessSurfacesRemainCoherentAndReadOnly()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem();
        var origin = new Planet(Guid.NewGuid(), system.Id, "Asterion", 1, PlanetType.Terran, 120, PlanetColonizationStatus.Colonized);
        var destination = new Planet(Guid.NewGuid(), system.Id, "Nereid", 2, PlanetType.Ice, 80);
        var group = OrbitalGroup.CreateStationed(civilizationId, origin.Id, origin.Id, SpaceAssetType.ScoutCraft, 5);
        group.Reserve();
        var transfer = OrbitalTransfer.CreatePlanned(
            civilizationId,
            group.Id,
            origin.Id,
            destination.Id,
            2,
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1));
        var stockpile = PlanetResourceStockpile.Create(origin.Id);
        stockpile.Increase(ResourceType.Credits, 100);
        stockpile.Increase(ResourceType.Gas, 50);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().AddRange(origin, destination);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(origin.Id, civilizationId));
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();

        var planetVisualService = new PlanetVisualStateService(dbContext);
        var systemVisualService = new SystemVisualStateService(dbContext, planetVisualService);
        var strategicMap = await new StrategicMapService(dbContext, systemVisualService, new MapVisibilityService(dbContext))
            .GetAsync(new GetStrategicMapRequest(civilizationId));
        var systemVisual = await systemVisualService.GetAsync(new GetSystemVisualStateRequest(system.Id));
        var fleetUiState = await new DevFleetUiStateService(
                dbContext,
                new FleetOperationalOverviewService(dbContext),
                new DevFleetActionManifestService())
            .GetAsync(new GetDevFleetUiStateRequest(civilizationId));
        var mapManifest = new DevStrategicMapActionManifestService().Get();

        var mapSystem = Assert.Single(strategicMap.Systems);
        Assert.Equal(system.Id, mapSystem.SystemId);
        Assert.Equal(MapVisibilityLevel.Visible, mapSystem.VisibilityLevel);
        Assert.True(mapSystem.IsVisible);
        Assert.Contains(mapSystem.Planets, x => x.PlanetId == origin.Id && x.IsOwnedByRequestingCivilization);
        Assert.Contains(mapSystem.Planets, x => x.PlanetId == destination.Id);
        Assert.Equal(group.Id, Assert.Single(mapSystem.FleetPresence).OrbitalGroupId);
        Assert.Equal(transfer.Id, Assert.Single(mapSystem.TransferOverlays).TransferId);
        Assert.Contains(strategicMap.RouteFuelNotes, x => x.ActionKey == "fleet.travel.estimate" && x.RequiresDestination);

        Assert.True(systemVisual.Succeeded);
        Assert.NotNull(systemVisual.VisualState);
        Assert.Equal(system.Id, systemVisual.VisualState.SystemId);
        Assert.Contains(systemVisual.VisualState.LayoutHints, x => x.PlanetId == origin.Id && x.OrbitalSlot == 1);
        Assert.Contains(systemVisual.VisualState.Planets, x => x.PlanetId == destination.Id);
        Assert.Equal(group.Id, Assert.Single(systemVisual.VisualState.OrbitalGroupMarkers).OrbitalGroupId);

        var uiGroup = Assert.Single(fleetUiState.Groups);
        Assert.Equal(group.Id, uiGroup.Id);
        Assert.Equal(transfer.Id, uiGroup.ActiveTransfer?.Id);
        Assert.False(uiGroup.RouteFuelReadiness.CanRequestTravelEstimate);
        Assert.Null(uiGroup.RouteFuelReadiness.RouteProfile);
        Assert.Null(uiGroup.RouteFuelReadiness.FuelReadiness);
        Assert.Contains(fleetUiState.ActionHints, x => x.ActionKey == "fleet.uiState.read" && x.IsReadOnly);
        Assert.Contains(fleetUiState.ResourceContexts, x => x.PlanetId == origin.Id);

        Assert.Contains(mapManifest.Actions, x => x.ActionKey == "strategicMap.read" && x.Route == "/api/dev/strategic-map");
        Assert.Contains(mapManifest.Actions, x => x.ActionKey == "visual.system.read" && x.RequiredFields.Any(field => field.Name == "systemId"));
        Assert.Contains(mapManifest.Actions, x => x.ActionKey == "visual.planet.read" && x.RequiredFields.Any(field => field.Name == "planetId"));
        Assert.Contains(mapManifest.Actions, x => x.ActionKey == "fleet.uiState.read" && x.Route == "/api/dev/fleets/ui-state");
        Assert.Contains(mapManifest.Actions, x => x.ActionKey == "fleet.actionManifest.read" && x.Route == "/api/dev/fleets/action-manifest");

        AssertReadSurfacesDidNotMutateState(dbContext, group.Id, transfer.Id, origin.Id);
        AssertNoHeavyRenderOrFutureGameplayContractData();
    }

    private static void AssertReadSurfacesDidNotMutateState(
        VoidEmpiresDbContext dbContext,
        Guid groupId,
        Guid transferId,
        Guid stockpilePlanetId)
    {
        var stockpile = dbContext.PlanetResourceStockpiles.AsNoTracking().Single(x => x.PlanetId == stockpilePlanetId);
        var group = dbContext.Set<OrbitalGroup>().AsNoTracking().Single(x => x.Id == groupId);
        var transfer = dbContext.Set<OrbitalTransfer>().AsNoTracking().Single(x => x.Id == transferId);

        Assert.Equal(100, stockpile.Credits);
        Assert.Equal(50, stockpile.Gas);
        Assert.Equal(OrbitalGroupStatus.Reserved, group.Status);
        Assert.Equal(5, group.Quantity);
        Assert.Equal(OrbitalTransferStatus.Planned, transfer.Status);
        Assert.DoesNotContain(dbContext.ChangeTracker.Entries(), x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);
    }

    private static void AssertNoHeavyRenderOrFutureGameplayContractData()
    {
        var contractTypes = new[]
        {
            typeof(GetStrategicMapResult),
            typeof(StrategicMapSystemDto),
            typeof(StrategicMapPlanetDto),
            typeof(StrategicMapFleetPresenceDto),
            typeof(StrategicMapTransferOverlayDto),
            typeof(SystemVisualStateDto),
            typeof(PlanetVisualStateDto),
            typeof(GetDevStrategicMapActionManifestResult)
        };
        var blockedTerms = new[] { "Mesh", "Texture", "Binary", "Shader", "RouteGraph", "Pathfinding", "Combat", "Interception" };

        foreach (var property in contractTypes.SelectMany(type => type.GetProperties()))
        {
            Assert.DoesNotContain(blockedTerms, term => property.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
    }

    private static SolarSystem CreateSystem()
    {
        var systemId = Guid.NewGuid();
        var star = new Star(Guid.NewGuid(), systemId, "Helios", StarType.YellowDwarf);
        return new SolarSystem(systemId, Guid.NewGuid(), "Helios Prime", new GalaxyCoordinates(1, 2, 3), star);
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
