using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class StrategicMapServiceTests
{
    [Fact]
    public async Task GetAsyncReturnsEmptyMapForCivilizationWithNoRelevantSystems()
    {
        await using var dbContext = CreateDbContext();
        var result = await CreateService(dbContext).GetAsync(new GetStrategicMapRequest(Guid.NewGuid()));

        Assert.Empty(result.Systems);
        Assert.Contains(result.RouteFuelNotes, x => x.RequiresDestination && x.ActionKey == "fleet.travel.estimate");
    }

    [Fact]
    public async Task GetAsyncReturnsOwnedSystemPlanetVisualAndLayoutSummary()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Helios", 1, 2, 3);
        var planet = new Planet(Guid.NewGuid(), system.Id, "Asterion", 2, PlanetType.Terran, 120, PlanetColonizationStatus.Colonized);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().Add(planet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(planet.Id, civilizationId));
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetStrategicMapRequest(civilizationId));

        var mapSystem = Assert.Single(result.Systems);
        Assert.Equal(system.Id, mapSystem.SystemId);
        Assert.Equal("Helios", mapSystem.SystemName);
        Assert.Equal(1, mapSystem.CoordinateX);
        Assert.Equal(StarType.YellowDwarf, mapSystem.StarType);
        Assert.Equal(MapVisibilityLevel.Visible, mapSystem.VisibilityLevel);
        Assert.Equal(MapVisibilityReason.SystemContainsOwnedPlanet, mapSystem.VisibilityReason);
        Assert.True(mapSystem.IsVisible);
        Assert.True(mapSystem.IsOwnedByRequestingCivilization);
        AssertAvailable(mapSystem.Commands, "strategicMap.system.view");
        var mapPlanet = Assert.Single(mapSystem.Planets);
        Assert.Equal(planet.Id, mapPlanet.PlanetId);
        Assert.True(mapPlanet.IsOwnedByRequestingCivilization);
        Assert.Equal(MapVisibilityLevel.Owned, mapPlanet.VisibilityLevel);
        Assert.Equal(MapVisibilityReason.OwnedPlanet, mapPlanet.VisibilityReason);
        Assert.True(mapPlanet.IsVisible);
        AssertAvailable(mapPlanet.Commands, "strategicMap.planet.viewDetail");
        AssertBlocked(mapPlanet.Commands, "fleet.travel.estimate", StrategicMapCommandBlockReason.NoFleetContext);
        Assert.Equal(civilizationId, mapPlanet.CivilizationId);
        Assert.Equal(2, mapPlanet.OrbitalSlot);
        Assert.Equal(7.5f, mapPlanet.OrbitRadius);
        Assert.Equal(PlanetColonizationStatus.Colonized, mapPlanet.ColonizationStatus);
    }

    [Fact]
    public async Task GetAsyncDoesNotExposeOtherCivilizationOwnershipOrFleetPresence()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var system = CreateSystem("Shared", 0, 0, 0);
        var ownedPlanet = new Planet(Guid.NewGuid(), system.Id, "Owned", 1, PlanetType.Terran, 100);
        var otherPlanet = new Planet(Guid.NewGuid(), system.Id, "Other", 2, PlanetType.Desert, 90);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().AddRange(ownedPlanet, otherPlanet);
        dbContext.Set<PlanetOwnership>().AddRange(
            PlanetOwnership.Create(ownedPlanet.Id, civilizationId),
            PlanetOwnership.Create(otherPlanet.Id, otherCivilizationId));
        dbContext.Set<PlanetBuilding>().AddRange(
            PlanetBuilding.Create(otherPlanet.Id, BuildingType.CommandCenter, 1, 5),
            PlanetBuilding.Create(otherPlanet.Id, BuildingType.MetalMine, 1, 5),
            PlanetBuilding.Create(otherPlanet.Id, BuildingType.DefenseGrid, 1, 5));
        dbContext.Set<OrbitalGroup>().Add(OrbitalGroup.CreateStationed(
            otherCivilizationId,
            otherPlanet.Id,
            otherPlanet.Id,
            SpaceAssetType.ScoutCraft,
            5));
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetStrategicMapRequest(civilizationId));

        var mapSystem = Assert.Single(result.Systems);
        var otherMapPlanet = mapSystem.Planets.Single(x => x.PlanetId == otherPlanet.Id);
        Assert.False(otherMapPlanet.IsOwnedByRequestingCivilization);
        Assert.Null(otherMapPlanet.CivilizationId);
        Assert.Equal(MapVisibilityLevel.Visible, otherMapPlanet.VisibilityLevel);
        Assert.True(otherMapPlanet.IsVisible);
        Assert.Equal(0f, otherMapPlanet.ColonizationIntensity);
        Assert.Equal(0f, otherMapPlanet.UrbanIntensity);
        Assert.Equal(0f, otherMapPlanet.IndustrialIntensity);
        Assert.Equal(0f, otherMapPlanet.MilitaryIntensity);
        Assert.Equal(0f, otherMapPlanet.OrbitalPresenceIntensity);
        Assert.Empty(mapSystem.FleetPresence);
    }

    [Fact]
    public async Task GetAsyncReturnsCivilizationFleetPresenceAndActiveTransferOverlays()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Transit", 4, 5, 6);
        var destinationSystem = CreateSystem("Unknown Destination", 7, 8, 9);
        var origin = new Planet(Guid.NewGuid(), system.Id, "Origin", 1, PlanetType.Oceanic, 100);
        var destination = new Planet(Guid.NewGuid(), destinationSystem.Id, "Destination", 1, PlanetType.Ice, 80);
        var group = OrbitalGroup.CreateStationed(civilizationId, origin.Id, origin.Id, SpaceAssetType.CargoCraft, 3);
        group.Reserve();
        var transfer = OrbitalTransfer.CreatePlanned(
            civilizationId,
            group.Id,
            origin.Id,
            destination.Id,
            2,
            new DateTime(2026, 5, 31, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 5, 31, 12, 0, 0, DateTimeKind.Utc));
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<SolarSystem>().Add(destinationSystem);
        dbContext.Set<Planet>().AddRange(origin, destination);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(origin.Id, civilizationId));
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetStrategicMapRequest(civilizationId));

        var mapSystem = result.Systems.Single(x => x.SystemId == system.Id);
        var unknownDestinationSystem = result.Systems.Single(x => x.SystemId == destinationSystem.Id);
        Assert.Equal(MapVisibilityLevel.Unknown, unknownDestinationSystem.VisibilityLevel);
        Assert.False(unknownDestinationSystem.IsVisible);
        AssertBlocked(unknownDestinationSystem.Commands, "strategicMap.system.view", StrategicMapCommandBlockReason.NotVisible);
        var unknownDestinationPlanet = Assert.Single(unknownDestinationSystem.Planets);
        Assert.Equal(MapVisibilityLevel.Unknown, unknownDestinationPlanet.VisibilityLevel);
        AssertBlocked(unknownDestinationPlanet.Commands, "strategicMap.planet.viewDetail", StrategicMapCommandBlockReason.Unknown);
        var presence = Assert.Single(mapSystem.FleetPresence);
        Assert.Equal(group.Id, presence.OrbitalGroupId);
        Assert.Equal(origin.Id, presence.PlanetId);
        Assert.Equal(SpaceAssetType.CargoCraft, presence.AssetType);
        Assert.Equal(OrbitalGroupStatus.Reserved, presence.Status);
        var overlay = Assert.Single(mapSystem.TransferOverlays);
        Assert.Equal(transfer.Id, overlay.TransferId);
        Assert.Equal(2, overlay.AbstractDistanceUnits);
        Assert.Equal(destination.Id, overlay.DestinationPlanetId);
        Assert.Equal(OrbitalTransferStatus.Planned, overlay.Status);
        var originPlanet = mapSystem.Planets.Single(x => x.PlanetId == origin.Id);
        AssertAvailable(originPlanet.Commands, "fleet.travel.estimate");
        AssertAvailable(originPlanet.Commands, "fleet.transfer.create");
        Assert.Contains("existing fleet command path", originPlanet.Commands.Single(x => x.ActionKey == "fleet.transfer.create").Note, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAsyncDoesNotMutatePersistedState()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Stable", 7, 8, 9);
        var planet = new Planet(Guid.NewGuid(), system.Id, "Quiet", 1, PlanetType.Barren, 70);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().Add(planet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(planet.Id, civilizationId));
        await dbContext.SaveChangesAsync();
        var ownershipCount = await dbContext.Set<PlanetOwnership>().CountAsync();

        _ = await CreateService(dbContext).GetAsync(new GetStrategicMapRequest(civilizationId));

        Assert.Equal(0, dbContext.ChangeTracker.Entries().Count(x => x.State != EntityState.Unchanged));
        Assert.Equal(ownershipCount, await dbContext.Set<PlanetOwnership>().CountAsync());
    }

    private static StrategicMapService CreateService(VoidEmpiresDbContext dbContext) =>
        new(
            dbContext,
            new SystemVisualStateService(dbContext, new PlanetVisualStateService(dbContext)),
            new MapVisibilityService(dbContext));

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
