using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class InterceptionOpportunityServiceTests
{
    [Fact]
    public async Task GetAsyncReturnsOwnTransferAsObservedOwnTransfer()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var homeSystem = CreateSystem("Home");
        var destinationSystem = CreateSystem("Destination");
        var homePlanet = new Planet(Guid.NewGuid(), homeSystem.Id, "Anchor", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var destinationPlanet = new Planet(Guid.NewGuid(), destinationSystem.Id, "Forward", 1, PlanetType.Oceanic, 90);
        var group = OrbitalGroup.CreateStationed(civilizationId, homePlanet.Id, homePlanet.Id, SpaceAssetType.CargoCraft, 2);
        group.Reserve();
        var transfer = CreateTransfer(group, destinationPlanet.Id);
        dbContext.AddRange(homeSystem, destinationSystem, homePlanet, destinationPlanet, group, transfer, PlanetOwnership.Create(homePlanet.Id, civilizationId));
        await dbContext.SaveChangesAsync();

        var opportunity = Assert.Single((await CreateService(dbContext).GetAsync(new GetInterceptionOpportunitiesRequest(civilizationId))).Opportunities);

        Assert.Equal(transfer.Id, opportunity.TransferId);
        Assert.Equal(InterceptionOpportunityStatus.ObservedOwnTransfer, opportunity.OpportunityStatus);
        Assert.Equal([InterceptionOpportunityBlockReason.SelfObservedTransfer], opportunity.BlockReasons);
        Assert.False(opportunity.HasFriendlyInterceptorContext);
    }

    [Fact]
    public async Task GetAsyncDoesNotExposeForeignTransferWithoutDetectionOrVisibility()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var homeSystem = CreateSystem("Home");
        var hiddenSystem = CreateSystem("Hidden");
        var homePlanet = new Planet(Guid.NewGuid(), homeSystem.Id, "Bastion", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var hiddenOrigin = new Planet(Guid.NewGuid(), hiddenSystem.Id, "Shadow", 1, PlanetType.Desert, 90);
        var hiddenDestination = new Planet(Guid.NewGuid(), hiddenSystem.Id, "Veil", 2, PlanetType.Barren, 60);
        var foreignGroup = OrbitalGroup.CreateStationed(otherCivilizationId, hiddenOrigin.Id, hiddenOrigin.Id, SpaceAssetType.ScoutCraft, 2);
        foreignGroup.Reserve();
        var foreignTransfer = CreateTransfer(foreignGroup, hiddenDestination.Id);
        dbContext.AddRange(
            homeSystem,
            hiddenSystem,
            homePlanet,
            hiddenOrigin,
            hiddenDestination,
            foreignGroup,
            foreignTransfer,
            PlanetOwnership.Create(homePlanet.Id, civilizationId));
        await dbContext.SaveChangesAsync();

        var opportunities = (await CreateService(dbContext).GetAsync(new GetInterceptionOpportunitiesRequest(civilizationId))).Opportunities;

        Assert.Empty(opportunities);
    }

    [Fact]
    public async Task GetAsyncReturnsDetectedForeignTransferOpportunityWhenFriendlyContextExists()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var system = CreateSystem("Observed");
        var ownedPlanet = new Planet(Guid.NewGuid(), system.Id, "Anchor", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var foreignOrigin = new Planet(Guid.NewGuid(), system.Id, "Intrusion One", 2, PlanetType.Desert, 80);
        var foreignDestination = new Planet(Guid.NewGuid(), system.Id, "Intrusion Two", 3, PlanetType.Barren, 70);
        var friendlyGroup = OrbitalGroup.CreateStationed(civilizationId, ownedPlanet.Id, ownedPlanet.Id, SpaceAssetType.ScoutCraft, 1);
        var foreignGroup = OrbitalGroup.CreateStationed(otherCivilizationId, foreignOrigin.Id, foreignOrigin.Id, SpaceAssetType.CargoCraft, 2);
        foreignGroup.Reserve();
        var foreignTransfer = CreateTransfer(foreignGroup, foreignDestination.Id);
        dbContext.AddRange(
            system,
            ownedPlanet,
            foreignOrigin,
            foreignDestination,
            friendlyGroup,
            foreignGroup,
            foreignTransfer,
            PlanetOwnership.Create(ownedPlanet.Id, civilizationId));
        await dbContext.SaveChangesAsync();

        var opportunity = Assert.Single((await CreateService(dbContext).GetAsync(new GetInterceptionOpportunitiesRequest(civilizationId))).Opportunities);

        Assert.Equal(foreignTransfer.Id, opportunity.TransferId);
        Assert.Equal(InterceptionOpportunityStatus.DetectedOpportunity, opportunity.OpportunityStatus);
        Assert.Empty(opportunity.BlockReasons);
        Assert.True(opportunity.HasFriendlyInterceptorContext);
    }

    [Fact]
    public async Task GetAsyncBlocksDetectedForeignTransferWhenNoFriendlyInterceptorContextExists()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var system = CreateSystem("Observed");
        var ownedPlanet = new Planet(Guid.NewGuid(), system.Id, "Anchor", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var foreignOrigin = new Planet(Guid.NewGuid(), system.Id, "Intrusion One", 2, PlanetType.Desert, 80);
        var foreignDestination = new Planet(Guid.NewGuid(), system.Id, "Intrusion Two", 3, PlanetType.Barren, 70);
        var foreignGroup = OrbitalGroup.CreateStationed(otherCivilizationId, foreignOrigin.Id, foreignOrigin.Id, SpaceAssetType.CargoCraft, 2);
        foreignGroup.Reserve();
        var foreignTransfer = CreateTransfer(foreignGroup, foreignDestination.Id);
        dbContext.AddRange(
            system,
            ownedPlanet,
            foreignOrigin,
            foreignDestination,
            foreignGroup,
            foreignTransfer,
            PlanetOwnership.Create(ownedPlanet.Id, civilizationId));
        await dbContext.SaveChangesAsync();

        var opportunity = Assert.Single((await CreateService(dbContext).GetAsync(new GetInterceptionOpportunitiesRequest(civilizationId))).Opportunities);

        Assert.Equal(InterceptionOpportunityStatus.Blocked, opportunity.OpportunityStatus);
        Assert.Equal([InterceptionOpportunityBlockReason.NoFriendlyInterceptorContext], opportunity.BlockReasons);
        Assert.False(opportunity.HasFriendlyInterceptorContext);
    }

    [Fact]
    public async Task GetAsyncRemainsReadOnly()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Stable");
        var ownedPlanet = new Planet(Guid.NewGuid(), system.Id, "Anchor", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var otherOrigin = new Planet(Guid.NewGuid(), system.Id, "Foreign A", 2, PlanetType.Desert, 80);
        var otherDestination = new Planet(Guid.NewGuid(), system.Id, "Foreign B", 3, PlanetType.Barren, 70);
        var friendlyGroup = OrbitalGroup.CreateStationed(civilizationId, ownedPlanet.Id, ownedPlanet.Id, SpaceAssetType.ScoutCraft, 1);
        var foreignGroup = OrbitalGroup.CreateStationed(Guid.NewGuid(), otherOrigin.Id, otherOrigin.Id, SpaceAssetType.CargoCraft, 2);
        foreignGroup.Reserve();
        var foreignTransfer = CreateTransfer(foreignGroup, otherDestination.Id);
        dbContext.AddRange(
            system,
            ownedPlanet,
            otherOrigin,
            otherDestination,
            friendlyGroup,
            foreignGroup,
            foreignTransfer,
            PlanetOwnership.Create(ownedPlanet.Id, civilizationId));
        await dbContext.SaveChangesAsync();
        var counts = (
            await dbContext.Set<OrbitalGroup>().CountAsync(),
            await dbContext.Set<OrbitalTransfer>().CountAsync(),
            await dbContext.Set<PlanetOwnership>().CountAsync());

        _ = await CreateService(dbContext).GetAsync(new GetInterceptionOpportunitiesRequest(civilizationId));

        Assert.Equal(0, dbContext.ChangeTracker.Entries().Count(x => x.State != EntityState.Unchanged));
        Assert.Equal(counts, (
            await dbContext.Set<OrbitalGroup>().CountAsync(),
            await dbContext.Set<OrbitalTransfer>().CountAsync(),
            await dbContext.Set<PlanetOwnership>().CountAsync()));
    }

    private static InterceptionOpportunityService CreateService(VoidEmpiresDbContext dbContext)
    {
        var mapVisibilityService = new MapVisibilityService(dbContext);
        var detectionCoverageService = new DetectionCoverageService(dbContext, new SensorProfileService(dbContext));
        var fleetOverviewService = new FleetOperationalOverviewService(dbContext);
        return new InterceptionOpportunityService(dbContext, mapVisibilityService, detectionCoverageService, fleetOverviewService);
    }

    private static OrbitalTransfer CreateTransfer(OrbitalGroup group, Guid destinationPlanetId) =>
        OrbitalTransfer.CreatePlanned(
            group.CivilizationId,
            group.Id,
            group.CurrentPlanetId,
            destinationPlanetId,
            1,
            new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 6, 1, 13, 0, 0, DateTimeKind.Utc));

    private static SolarSystem CreateSystem(string name)
    {
        var systemId = Guid.NewGuid();
        return new SolarSystem(
            systemId,
            Guid.NewGuid(),
            name,
            new GalaxyCoordinates(1, 2, 3),
            new Star(Guid.NewGuid(), systemId, $"{name} Star", StarType.YellowDwarf));
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
