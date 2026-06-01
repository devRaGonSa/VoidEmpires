using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class InterceptionReadinessSmokeTests
{
    [Fact]
    public async Task InterceptionReadinessSurfacesRemainCoherentConservativeAndReadOnly()
    {
        await using var dbContext = CreateDbContext();
        var requestedAtUtc = new DateTime(2026, 6, 1, 18, 0, 0, DateTimeKind.Utc);
        var player = PlayerProfile.Create(Guid.NewGuid().ToString(), "Interception Smoke");
        var civilization = Civilization.Create(player.Id, "Interception Readiness", CivilizationArchetype.Exploratory);
        var otherCivilizationId = Guid.NewGuid();
        var homeSystem = CreateSystem("Home", 1, 2, 3);
        var ownDestinationSystem = CreateSystem("Own Frontier", 4, 5, 6);
        var hiddenForeignSystem = CreateSystem("Hidden Foreign", 7, 8, 9);
        var homePlanet = new Planet(Guid.NewGuid(), homeSystem.Id, "Bastion", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var ownDestinationPlanet = new Planet(Guid.NewGuid(), ownDestinationSystem.Id, "Relay", 1, PlanetType.Oceanic, 90);
        var hiddenForeignOrigin = new Planet(Guid.NewGuid(), hiddenForeignSystem.Id, "Shade", 1, PlanetType.Desert, 80);
        var hiddenForeignDestination = new Planet(Guid.NewGuid(), hiddenForeignSystem.Id, "Veil", 2, PlanetType.Barren, 70);
        var stockpile = PlanetResourceStockpile.Create(homePlanet.Id);
        stockpile.Increase(ResourceType.Credits, 50);
        stockpile.Increase(ResourceType.Gas, 20);
        var ownTransferGroup = OrbitalGroup.CreateStationed(civilization.Id, homePlanet.Id, homePlanet.Id, SpaceAssetType.CargoCraft, 3);
        ownTransferGroup.Reserve();
        var homeScout = OrbitalGroup.CreateStationed(civilization.Id, homePlanet.Id, homePlanet.Id, SpaceAssetType.ScoutCraft, 1);
        var hiddenForeignGroup = OrbitalGroup.CreateStationed(otherCivilizationId, hiddenForeignOrigin.Id, hiddenForeignOrigin.Id, SpaceAssetType.CargoCraft, 2);
        hiddenForeignGroup.Reserve();
        var ownTransfer = OrbitalTransfer.CreatePlanned(
            civilization.Id,
            ownTransferGroup.Id,
            homePlanet.Id,
            ownDestinationPlanet.Id,
            2,
            requestedAtUtc,
            requestedAtUtc.AddHours(2));
        var hiddenForeignTransfer = OrbitalTransfer.CreatePlanned(
            otherCivilizationId,
            hiddenForeignGroup.Id,
            hiddenForeignOrigin.Id,
            hiddenForeignDestination.Id,
            1,
            requestedAtUtc.AddMinutes(15),
            requestedAtUtc.AddHours(1));

        dbContext.PlayerProfiles.Add(player);
        dbContext.Civilizations.Add(civilization);
        dbContext.Set<SolarSystem>().AddRange(homeSystem, ownDestinationSystem, hiddenForeignSystem);
        dbContext.Set<Planet>().AddRange(homePlanet, ownDestinationPlanet, hiddenForeignOrigin, hiddenForeignDestination);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(homePlanet.Id, civilization.Id));
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        dbContext.Set<OrbitalGroup>().AddRange(ownTransferGroup, homeScout, hiddenForeignGroup);
        dbContext.Set<OrbitalTransfer>().AddRange(ownTransfer, hiddenForeignTransfer);
        await dbContext.SaveChangesAsync();

        var sensorProfileService = new SensorProfileService(dbContext);
        var detectionCoverageService = new DetectionCoverageService(dbContext, sensorProfileService);
        var visibilityService = new MapVisibilityService(dbContext);
        var fleetOverviewService = new FleetOperationalOverviewService(dbContext);
        var interceptionService = new InterceptionOpportunityService(dbContext, visibilityService, detectionCoverageService, fleetOverviewService);
        var strategicMapService = new StrategicMapService(
            dbContext,
            new SystemVisualStateService(dbContext, new PlanetVisualStateService(dbContext)),
            visibilityService,
            sensorProfileService,
            detectionCoverageService,
            interceptionService);
        var fleetUiStateService = new DevFleetUiStateService(
            dbContext,
            fleetOverviewService,
            new DevFleetActionManifestService(),
            interceptionService);

        var expectedCounts = (
            await dbContext.Set<OrbitalGroup>().CountAsync(),
            await dbContext.Set<OrbitalTransfer>().CountAsync(),
            await dbContext.Set<PlanetOwnership>().CountAsync(),
            await dbContext.PlanetResourceStockpiles.CountAsync(),
            await dbContext.ExplorationKnowledge.CountAsync(),
            await dbContext.ExplorationMissions.CountAsync());

        var detectionCoverage = await detectionCoverageService.GetAsync(new GetDetectionCoverageRequest(civilization.Id));
        Assert.Contains(detectionCoverage.Coverages, x => x.SourceKind == DetectionCoverageSourceKind.Planet && x.SourcePlanetId == homePlanet.Id);

        var interceptionOpportunities = await interceptionService.GetAsync(new GetInterceptionOpportunitiesRequest(civilization.Id));
        var ownOpportunity = Assert.Single(interceptionOpportunities.Opportunities);
        Assert.Equal(ownTransfer.Id, ownOpportunity.TransferId);
        Assert.Equal(InterceptionOpportunityStatus.ObservedOwnTransfer, ownOpportunity.OpportunityStatus);
        Assert.Equal([InterceptionOpportunityBlockReason.SelfObservedTransfer], ownOpportunity.BlockReasons);
        Assert.DoesNotContain(interceptionOpportunities.Opportunities, x => x.TransferId == hiddenForeignTransfer.Id);

        var strategicMap = await strategicMapService.GetAsync(new GetStrategicMapRequest(civilization.Id));
        Assert.Contains(strategicMap.InterceptionNotes, x => x.Note.Contains("read-only", StringComparison.OrdinalIgnoreCase));
        var homeMapSystem = strategicMap.Systems.Single(x => x.SystemId == homeSystem.Id);
        var ownOverlay = Assert.Single(homeMapSystem.TransferOverlays);
        Assert.Equal(ownTransfer.Id, ownOverlay.TransferId);
        Assert.NotNull(ownOverlay.InterceptionReadiness);
        Assert.Equal(InterceptionOpportunityStatus.ObservedOwnTransfer, ownOverlay.InterceptionReadiness.OpportunityStatus);
        Assert.Equal([InterceptionOpportunityBlockReason.SelfObservedTransfer], ownOverlay.InterceptionReadiness.BlockReasons);
        Assert.DoesNotContain(strategicMap.Systems.SelectMany(x => x.TransferOverlays), x => x.TransferId == hiddenForeignTransfer.Id);

        var fleetUiState = await fleetUiStateService.GetAsync(new GetDevFleetUiStateRequest(civilization.Id));
        Assert.Contains(fleetUiState.InterceptionNotes, x => x.Note.Contains("not implemented", StringComparison.OrdinalIgnoreCase));
        var ownUiTransfer = fleetUiState.Groups.Single(x => x.Id == ownTransferGroup.Id).ActiveTransfer;
        Assert.NotNull(ownUiTransfer);
        Assert.NotNull(ownUiTransfer.InterceptionReadiness);
        Assert.Equal(InterceptionOpportunityStatus.ObservedOwnTransfer, ownUiTransfer.InterceptionReadiness.OpportunityStatus);
        Assert.Equal([InterceptionOpportunityBlockReason.SelfObservedTransfer], ownUiTransfer.InterceptionReadiness.BlockReasons);

        var strategicManifest = new DevStrategicMapActionManifestService().Get().Actions;
        Assert.Contains(strategicManifest, x => x.ActionKey == "interception.opportunity.read" && x.Route == "/api/dev/strategic-map/interception-opportunities" && x.IsReadOnly);
        var fleetManifest = new DevFleetActionManifestService().Get().Actions;
        Assert.Contains(fleetManifest, x => x.ActionKey == "fleet.interception.readiness.read" && x.Route == "/api/dev/strategic-map/interception-opportunities" && x.IsReadOnly);

        Assert.Equal(0, dbContext.ChangeTracker.Entries().Count(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted));
        Assert.Equal(expectedCounts, (
            await dbContext.Set<OrbitalGroup>().CountAsync(),
            await dbContext.Set<OrbitalTransfer>().CountAsync(),
            await dbContext.Set<PlanetOwnership>().CountAsync(),
            await dbContext.PlanetResourceStockpiles.CountAsync(),
            await dbContext.ExplorationKnowledge.CountAsync(),
            await dbContext.ExplorationMissions.CountAsync()));
        AssertNoInterceptionExecutionSystems(dbContext);
    }

    private static void AssertNoInterceptionExecutionSystems(VoidEmpiresDbContext dbContext)
    {
        var entityNames = dbContext.Model.GetEntityTypes().Select(x => x.ClrType.Name).ToArray();
        var blockedTerms = new[]
        {
            "InterceptionState", "CombatResult", "Damage", "BattleResolution",
            "Espionage", "Diplomacy", "RouteGraph", "Pathfinding", "FinalUi"
        };

        foreach (var term in blockedTerms)
        {
            Assert.DoesNotContain(entityNames, x => x.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
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
