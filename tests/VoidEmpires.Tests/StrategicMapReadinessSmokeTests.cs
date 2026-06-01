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
        var otherCivilizationId = Guid.NewGuid();
        var system = CreateSystem();
        var destinationSystem = CreateSystem("Outer Veil", 8, 9, 10);
        var origin = new Planet(Guid.NewGuid(), system.Id, "Asterion", 1, PlanetType.Terran, 120, PlanetColonizationStatus.Colonized);
        var foreignPlanet = new Planet(Guid.NewGuid(), system.Id, "Vesper", 2, PlanetType.Desert, 90, PlanetColonizationStatus.Colonized);
        var destination = new Planet(Guid.NewGuid(), destinationSystem.Id, "Nereid", 1, PlanetType.Ice, 80);
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
        dbContext.Set<SolarSystem>().AddRange(system, destinationSystem);
        dbContext.Set<Planet>().AddRange(origin, foreignPlanet, destination);
        dbContext.Set<PlanetOwnership>().AddRange(
            PlanetOwnership.Create(origin.Id, civilizationId),
            PlanetOwnership.Create(foreignPlanet.Id, otherCivilizationId));
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();

        var planetVisualService = new PlanetVisualStateService(dbContext);
        var systemVisualService = new SystemVisualStateService(dbContext, planetVisualService);
        var visibility = await new MapVisibilityService(dbContext).GetAsync(new GetMapVisibilityRequest(civilizationId));
        var strategicMap = await new StrategicMapService(dbContext, systemVisualService, new MapVisibilityService(dbContext))
            .GetAsync(new GetStrategicMapRequest(civilizationId));
        var explorationPreview = await new ExplorationActionPreviewService(new MapVisibilityService(dbContext))
            .GetAsync(new GetExplorationActionPreviewRequest(civilizationId));
        var systemVisual = await systemVisualService.GetAsync(new GetSystemVisualStateRequest(system.Id));
        var fleetUiState = await new DevFleetUiStateService(
                dbContext,
                new FleetOperationalOverviewService(dbContext),
                new DevFleetActionManifestService())
            .GetAsync(new GetDevFleetUiStateRequest(civilizationId));
        var mapManifest = new DevStrategicMapActionManifestService().Get();

        var visibleSystem = visibility.Systems.Single(x => x.SystemId == system.Id);
        Assert.Equal(MapVisibilityLevel.Visible, visibleSystem.VisibilityLevel);
        Assert.Contains(visibleSystem.Planets, x => x.PlanetId == origin.Id && x.VisibilityLevel == MapVisibilityLevel.Owned);
        Assert.Contains(visibleSystem.Planets, x => x.PlanetId == foreignPlanet.Id && !x.IsOwnedByRequestingCivilization && x.CivilizationId is null);
        var unknownVisibilitySystem = visibility.Systems.Single(x => x.SystemId == destinationSystem.Id);
        Assert.Equal(MapVisibilityLevel.Unknown, unknownVisibilitySystem.VisibilityLevel);
        Assert.False(unknownVisibilitySystem.IsVisible);

        var mapSystem = strategicMap.Systems.Single(x => x.SystemId == system.Id);
        Assert.Equal(system.Id, mapSystem.SystemId);
        Assert.Equal(MapVisibilityLevel.Visible, mapSystem.VisibilityLevel);
        Assert.True(mapSystem.IsVisible);
        Assert.False(mapSystem.ExplorationPreview.CanPreviewExploration);
        Assert.Equal(ExplorationActionBlockReason.AlreadyVisible, mapSystem.ExplorationPreview.BlockReason);
        AssertAvailable(mapSystem.Commands, "strategicMap.system.view");
        AssertBlocked(mapSystem.Commands, "exploration.preview", StrategicMapCommandBlockReason.ExplorationPreviewUnavailable);
        var originMapPlanet = mapSystem.Planets.Single(x => x.PlanetId == origin.Id);
        Assert.True(originMapPlanet.IsOwnedByRequestingCivilization);
        Assert.Equal(MapVisibilityLevel.Owned, originMapPlanet.VisibilityLevel);
        Assert.Equal(ExplorationActionBlockReason.AlreadyOwned, originMapPlanet.ExplorationPreview.BlockReason);
        AssertAvailable(originMapPlanet.Commands, "strategicMap.planet.viewDetail");
        AssertBlocked(originMapPlanet.Commands, "exploration.preview", StrategicMapCommandBlockReason.ExplorationPreviewUnavailable);
        AssertAvailable(originMapPlanet.Commands, "fleet.travel.estimate");
        AssertAvailable(originMapPlanet.Commands, "fleet.transfer.create");
        var foreignMapPlanet = mapSystem.Planets.Single(x => x.PlanetId == foreignPlanet.Id);
        Assert.False(foreignMapPlanet.IsOwnedByRequestingCivilization);
        Assert.Null(foreignMapPlanet.CivilizationId);
        Assert.Equal(MapVisibilityLevel.Visible, foreignMapPlanet.VisibilityLevel);
        var unknownMapSystem = strategicMap.Systems.Single(x => x.SystemId == destinationSystem.Id);
        Assert.Equal(MapVisibilityLevel.Unknown, unknownMapSystem.VisibilityLevel);
        Assert.True(unknownMapSystem.ExplorationPreview.CanPreviewExploration);
        AssertAvailable(unknownMapSystem.Commands, "exploration.preview");
        AssertBlocked(unknownMapSystem.Commands, "strategicMap.system.view", StrategicMapCommandBlockReason.NotVisible);
        var unknownMapPlanet = Assert.Single(unknownMapSystem.Planets);
        Assert.Equal(destination.Id, unknownMapPlanet.PlanetId);
        Assert.True(unknownMapPlanet.ExplorationPreview.CanPreviewExploration);
        AssertAvailable(unknownMapPlanet.Commands, "exploration.preview");
        AssertBlocked(unknownMapPlanet.Commands, "strategicMap.planet.viewDetail", StrategicMapCommandBlockReason.Unknown);
        AssertBlocked(unknownMapPlanet.Commands, "fleet.transfer.create", StrategicMapCommandBlockReason.Unknown);
        Assert.Equal(group.Id, Assert.Single(mapSystem.FleetPresence).OrbitalGroupId);
        Assert.Equal(transfer.Id, Assert.Single(mapSystem.TransferOverlays).TransferId);
        Assert.Contains(strategicMap.RouteFuelNotes, x => x.ActionKey == "fleet.travel.estimate" && x.RequiresDestination);

        var previewDestinationSystem = explorationPreview.Systems.Single(x => x.SystemId == destinationSystem.Id);
        Assert.True(previewDestinationSystem.CanPreviewSystemExploration);
        Assert.Equal(ExplorationActionBlockReason.None, previewDestinationSystem.BlockReason);
        Assert.Contains(explorationPreview.Notes, x => x.ActionKey == "exploration.preview" && x.IsReadOnly);

        Assert.True(systemVisual.Succeeded);
        Assert.NotNull(systemVisual.VisualState);
        Assert.Equal(system.Id, systemVisual.VisualState.SystemId);
        Assert.Contains(systemVisual.VisualState.LayoutHints, x => x.PlanetId == origin.Id && x.OrbitalSlot == 1);
        Assert.Contains(systemVisual.VisualState.Planets, x => x.PlanetId == foreignPlanet.Id);
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
        Assert.Contains(mapManifest.Actions, x => x.ActionKey == "strategicMap.explorationPreview.read" && x.Route == "/api/dev/strategic-map/exploration-preview");
        Assert.Contains(mapManifest.Actions, x => x.ActionKey == "visual.system.read" && x.RequiredFields.Any(field => field.Name == "systemId"));
        Assert.Contains(mapManifest.Actions, x => x.ActionKey == "visual.planet.read" && x.RequiredFields.Any(field => field.Name == "planetId"));
        Assert.Contains(mapManifest.Actions, x => x.ActionKey == "fleet.uiState.read" && x.Route == "/api/dev/fleets/ui-state");
        Assert.Contains(mapManifest.Actions, x => x.ActionKey == "fleet.actionManifest.read" && x.Route == "/api/dev/fleets/action-manifest");

        AssertReadSurfacesDidNotMutateState(dbContext, group.Id, transfer.Id, origin.Id, 2, 3, 2);
        AssertNoHeavyRenderOrFutureGameplayContractData();
    }

    private static void AssertReadSurfacesDidNotMutateState(
        VoidEmpiresDbContext dbContext,
        Guid groupId,
        Guid transferId,
        Guid stockpilePlanetId,
        int expectedSystemCount,
        int expectedPlanetCount,
        int expectedOwnershipCount)
    {
        var stockpile = dbContext.PlanetResourceStockpiles.AsNoTracking().Single(x => x.PlanetId == stockpilePlanetId);
        var group = dbContext.Set<OrbitalGroup>().AsNoTracking().Single(x => x.Id == groupId);
        var transfer = dbContext.Set<OrbitalTransfer>().AsNoTracking().Single(x => x.Id == transferId);

        Assert.Equal(100, stockpile.Credits);
        Assert.Equal(50, stockpile.Gas);
        Assert.Equal(OrbitalGroupStatus.Reserved, group.Status);
        Assert.Equal(5, group.Quantity);
        Assert.Equal(OrbitalTransferStatus.Planned, transfer.Status);
        Assert.Equal(expectedSystemCount, dbContext.Set<SolarSystem>().AsNoTracking().Count());
        Assert.Equal(expectedPlanetCount, dbContext.Set<Planet>().AsNoTracking().Count());
        Assert.Equal(expectedOwnershipCount, dbContext.Set<PlanetOwnership>().AsNoTracking().Count());
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
        var blockedTerms = new[] { "Mesh", "Texture", "Binary", "Shader", "RouteGraph", "Pathfinding", "Combat", "Interception", "Fog", "Sensor", "Scanner" };

        foreach (var property in contractTypes.SelectMany(type => type.GetProperties()))
        {
            Assert.DoesNotContain(blockedTerms, term => property.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
    }

    private static void AssertAvailable(IReadOnlyList<StrategicMapCommandAvailabilityDto> commands, string actionKey)
    {
        var command = commands.Single(x => x.ActionKey == actionKey);
        Assert.True(command.IsAvailable);
        Assert.Equal(StrategicMapCommandBlockReason.None, command.BlockReason);
    }

    private static void AssertBlocked(
        IReadOnlyList<StrategicMapCommandAvailabilityDto> commands,
        string actionKey,
        StrategicMapCommandBlockReason reason)
    {
        var command = commands.Single(x => x.ActionKey == actionKey);
        Assert.False(command.IsAvailable);
        Assert.Equal(reason, command.BlockReason);
    }

    private static SolarSystem CreateSystem() => CreateSystem("Helios Prime", 1, 2, 3);

    private static SolarSystem CreateSystem(string name, int x, int y, int z)
    {
        var systemId = Guid.NewGuid();
        var star = new Star(Guid.NewGuid(), systemId, $"{name} Star", StarType.YellowDwarf);
        return new SolarSystem(systemId, Guid.NewGuid(), name, new GalaxyCoordinates(x, y, z), star);
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
