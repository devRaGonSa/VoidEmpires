using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Domain.Exploration;
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
        Assert.Empty(result.AlliancePacts);
        Assert.Empty(result.AllianceReadiness);
        Assert.Contains(result.RouteFuelNotes, x => x.RequiresDestination && x.ActionKey == "fleet.travel.estimate");
        Assert.Contains(result.InterceptionNotes, x => x.Note.Contains("read-only", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.AllianceNotes, x => x.Note.Contains("shared visibility", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.AlliancePactNotes, x => x.Note.Contains("active membership", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetAsyncReturnsOwnAllianceAndPactMetadataWithoutAddingAllianceVisibility()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var allyCivilizationId = Guid.NewGuid();
        var sharedAlliance = Alliance.Create("Void Council", "VC", AllianceStatus.Active, new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc));
        var allyAlliance = Alliance.Create("Star Accord", "STAR", AllianceStatus.Active, new DateTime(2026, 6, 1, 12, 1, 0, DateTimeKind.Utc));
        var alliedOnlySystem = CreateSystem("Ally Hidden", 8, 8, 8);
        var alliedPlanet = new Planet(Guid.NewGuid(), alliedOnlySystem.Id, "Ally Prime", 1, PlanetType.Terran, 100);
        dbContext.Alliances.AddRange(sharedAlliance, allyAlliance);
        dbContext.AllianceMemberships.AddRange(
            AllianceMembership.Create(sharedAlliance.Id, civilizationId, AllianceMembershipStatus.Active, AllianceMembershipRole.Officer, new DateTime(2026, 6, 1, 12, 5, 0, DateTimeKind.Utc)),
            AllianceMembership.Create(sharedAlliance.Id, allyCivilizationId, AllianceMembershipStatus.Active, AllianceMembershipRole.Member, new DateTime(2026, 6, 1, 12, 6, 0, DateTimeKind.Utc)));
        dbContext.AlliancePacts.Add(AlliancePact.Create(
            sharedAlliance.Id,
            allyAlliance.Id,
            AlliancePactType.NonAggression,
            AlliancePactStatus.Active,
            new DateTime(2026, 6, 1, 12, 10, 0, DateTimeKind.Utc)));
        dbContext.Set<SolarSystem>().Add(alliedOnlySystem);
        dbContext.Set<Planet>().Add(alliedPlanet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(alliedPlanet.Id, allyCivilizationId));
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetStrategicMapRequest(civilizationId));

        var alliance = Assert.Single(result.AllianceReadiness);
        var pact = Assert.Single(result.AlliancePacts);
        Assert.Equal(sharedAlliance.Id, alliance.AllianceId);
        Assert.Equal(civilizationId, alliance.Membership.CivilizationId);
        Assert.Equal("VC", alliance.Tag);
        Assert.Equal(sharedAlliance.Id, pact.SourceAlliance.AllianceId);
        Assert.Equal(allyAlliance.Id, pact.TargetAlliance.AllianceId);
        Assert.Equal(AlliancePactType.NonAggression, pact.PactType);
        Assert.Contains(result.AllianceNotes, x => x.Note.Contains("requesting civilization only", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.AlliancePactNotes, x => x.Note.Contains("does not add strategic map relevance", StringComparison.OrdinalIgnoreCase));
        Assert.Empty(result.Systems);
    }

    [Fact]
    public async Task GetAsyncKeepsDiplomaticContactsIndependentFromAllianceMembership()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var contactedCivilizationId = Guid.NewGuid();
        var alliance = Alliance.Create("Void Council", "VC", AllianceStatus.Active, new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc));
        dbContext.Alliances.Add(alliance);
        dbContext.AllianceMemberships.Add(AllianceMembership.Create(
            alliance.Id,
            civilizationId,
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Leader,
            new DateTime(2026, 6, 1, 12, 5, 0, DateTimeKind.Utc)));
        dbContext.DiplomaticContacts.Add(DiplomaticContact.Create(
            civilizationId,
            contactedCivilizationId,
            DiplomaticContactStatus.Friendly,
            new DateTime(2026, 6, 1, 12, 10, 0, DateTimeKind.Utc),
            "manual-dev"));
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetStrategicMapRequest(civilizationId));

        Assert.Single(result.AllianceReadiness);
        var contact = Assert.Single(result.DiplomaticContacts);
        Assert.Equal(contactedCivilizationId, contact.ContactedCivilizationId);
        Assert.DoesNotContain(result.AllianceReadiness, x => x.Membership.CivilizationId == contact.ContactedCivilizationId);
        Assert.Contains("does not imply alliance", contact.Note, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAsyncKeepsAlliancePactsIndependentFromDiplomaticContacts()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var contactedCivilizationId = Guid.NewGuid();
        var alliance = Alliance.Create("Void Council", "VC", AllianceStatus.Active, new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc));
        var partnerAlliance = Alliance.Create("Star Accord", "STAR", AllianceStatus.Active, new DateTime(2026, 6, 1, 12, 1, 0, DateTimeKind.Utc));
        dbContext.Alliances.AddRange(alliance, partnerAlliance);
        dbContext.AllianceMemberships.Add(AllianceMembership.Create(
            alliance.Id,
            civilizationId,
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Leader,
            new DateTime(2026, 6, 1, 12, 5, 0, DateTimeKind.Utc)));
        dbContext.AlliancePacts.Add(AlliancePact.Create(
            alliance.Id,
            partnerAlliance.Id,
            AlliancePactType.Cooperation,
            AlliancePactStatus.Proposed,
            new DateTime(2026, 6, 1, 12, 6, 0, DateTimeKind.Utc)));
        dbContext.DiplomaticContacts.Add(DiplomaticContact.Create(
            civilizationId,
            contactedCivilizationId,
            DiplomaticContactStatus.Friendly,
            new DateTime(2026, 6, 1, 12, 10, 0, DateTimeKind.Utc),
            "manual-dev"));
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetStrategicMapRequest(civilizationId));

        var pact = Assert.Single(result.AlliancePacts);
        var contact = Assert.Single(result.DiplomaticContacts);
        Assert.Equal(partnerAlliance.Id, pact.TargetAlliance.AllianceId);
        Assert.Equal(contactedCivilizationId, contact.ContactedCivilizationId);
        Assert.DoesNotContain(result.AlliancePacts, x => x.TargetAlliance.AllianceId == contact.ContactedCivilizationId);
        Assert.Contains(result.AlliancePactNotes, x => x.Note.Contains("authorization", StringComparison.OrdinalIgnoreCase));
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

        Assert.Contains(result.SensorNotes, x => x.Note.Contains("do not reveal visibility", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.DetectionNotes, x => x.Note.Contains("does not reveal unknown systems or planets", StringComparison.OrdinalIgnoreCase));
        var mapSystem = Assert.Single(result.Systems);
        Assert.Equal(system.Id, mapSystem.SystemId);
        Assert.Equal("Helios", mapSystem.SystemName);
        Assert.Equal(1, mapSystem.CoordinateX);
        Assert.Equal(StarType.YellowDwarf, mapSystem.StarType);
        Assert.Equal(MapVisibilityLevel.Visible, mapSystem.VisibilityLevel);
        Assert.Equal(MapVisibilityReason.SystemContainsOwnedPlanet, mapSystem.VisibilityReason);
        Assert.True(mapSystem.IsVisible);
        Assert.True(mapSystem.IsOwnedByRequestingCivilization);
        Assert.Equal(ExplorationActionBlockReason.AlreadyVisible, mapSystem.ExplorationPreview.BlockReason);
        AssertAvailable(mapSystem.Commands, "strategicMap.system.view");
        AssertBlocked(mapSystem.Commands, "exploration.preview", StrategicMapCommandBlockReason.ExplorationPreviewUnavailable);
        AssertBlocked(mapSystem.Commands, "exploration.mission.create", StrategicMapCommandBlockReason.ExplorationPreviewUnavailable);
        var mapPlanet = Assert.Single(mapSystem.Planets);
        Assert.Equal(planet.Id, mapPlanet.PlanetId);
        Assert.True(mapPlanet.IsOwnedByRequestingCivilization);
        Assert.Equal(MapVisibilityLevel.Owned, mapPlanet.VisibilityLevel);
        Assert.Equal(MapVisibilityReason.OwnedPlanet, mapPlanet.VisibilityReason);
        Assert.True(mapPlanet.IsVisible);
        Assert.Equal(ExplorationActionBlockReason.AlreadyOwned, mapPlanet.ExplorationPreview.BlockReason);
        AssertAvailable(mapPlanet.Commands, "strategicMap.planet.viewDetail");
        AssertBlocked(mapPlanet.Commands, "exploration.preview", StrategicMapCommandBlockReason.ExplorationPreviewUnavailable);
        AssertBlocked(mapPlanet.Commands, "exploration.mission.create", StrategicMapCommandBlockReason.ExplorationPreviewUnavailable);
        AssertBlocked(mapPlanet.Commands, "fleet.travel.estimate", StrategicMapCommandBlockReason.NoFleetContext);
        Assert.Equal(civilizationId, mapPlanet.CivilizationId);
        Assert.Equal(2, mapPlanet.OrbitalSlot);
        Assert.Equal(7.5f, mapPlanet.OrbitRadius);
        Assert.Equal(PlanetColonizationStatus.Colonized, mapPlanet.ColonizationStatus);
        Assert.Equal(SensorProfileClass.Orbital, Assert.Single(mapSystem.SensorProfiles).SensorClass);
        Assert.Equal(SensorProfileClass.Orbital, Assert.Single(mapPlanet.SensorProfiles).SensorClass);
        Assert.Equal(DetectionCoverageClass.Orbital, Assert.Single(mapSystem.DetectionCoverage).CoverageClass);
        Assert.Equal(DetectionCoverageSourceKind.Planet, Assert.Single(mapPlanet.DetectionCoverage).SourceKind);
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
        Assert.Equal(ExplorationActionBlockReason.AlreadyVisible, otherMapPlanet.ExplorationPreview.BlockReason);
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
        var remoteVisibleSystem = CreateSystem("Remote Visible", 6, 7, 8);
        var destinationSystem = CreateSystem("Unknown Destination", 7, 8, 9);
        var origin = new Planet(Guid.NewGuid(), system.Id, "Origin", 1, PlanetType.Oceanic, 100);
        var remoteVisiblePlanet = new Planet(Guid.NewGuid(), remoteVisibleSystem.Id, "Remote", 1, PlanetType.Terran, 95);
        var destination = new Planet(Guid.NewGuid(), destinationSystem.Id, "Destination", 1, PlanetType.Ice, 80);
        var group = OrbitalGroup.CreateStationed(civilizationId, origin.Id, origin.Id, SpaceAssetType.CargoCraft, 3);
        var scoutGroup = OrbitalGroup.CreateStationed(civilizationId, origin.Id, origin.Id, SpaceAssetType.ScoutCraft, 1);
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
        dbContext.Set<SolarSystem>().Add(remoteVisibleSystem);
        dbContext.Set<SolarSystem>().Add(destinationSystem);
        dbContext.Set<Planet>().AddRange(origin, remoteVisiblePlanet, destination);
        dbContext.Set<PlanetOwnership>().AddRange(
            PlanetOwnership.Create(origin.Id, civilizationId),
            PlanetOwnership.Create(remoteVisiblePlanet.Id, civilizationId));
        dbContext.Set<OrbitalGroup>().AddRange(group, scoutGroup);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetStrategicMapRequest(civilizationId));

        var mapSystem = result.Systems.Single(x => x.SystemId == system.Id);
        var remoteMapSystem = result.Systems.Single(x => x.SystemId == remoteVisibleSystem.Id);
        var unknownDestinationSystem = result.Systems.Single(x => x.SystemId == destinationSystem.Id);
        Assert.Equal(MapVisibilityLevel.Unknown, unknownDestinationSystem.VisibilityLevel);
        Assert.False(unknownDestinationSystem.IsVisible);
        Assert.True(unknownDestinationSystem.ExplorationPreview.CanPreviewExploration);
        AssertAvailable(unknownDestinationSystem.Commands, "exploration.preview");
        AssertAvailable(unknownDestinationSystem.Commands, "exploration.mission.create");
        AssertBlocked(unknownDestinationSystem.Commands, "strategicMap.system.view", StrategicMapCommandBlockReason.NotVisible);
        Assert.Empty(unknownDestinationSystem.SensorProfiles);
        Assert.Empty(unknownDestinationSystem.DetectionCoverage);
        var unknownDestinationPlanet = Assert.Single(unknownDestinationSystem.Planets);
        Assert.Equal(MapVisibilityLevel.Unknown, unknownDestinationPlanet.VisibilityLevel);
        Assert.Empty(unknownDestinationPlanet.SensorProfiles);
        Assert.Empty(unknownDestinationPlanet.DetectionCoverage);
        Assert.True(unknownDestinationPlanet.ExplorationPreview.CanPreviewExploration);
        AssertAvailable(unknownDestinationPlanet.Commands, "exploration.preview");
        AssertAvailable(unknownDestinationPlanet.Commands, "exploration.mission.create");
        Assert.Contains("mission creation", unknownDestinationPlanet.Commands.Single(x => x.ActionKey == "exploration.mission.create").Note, StringComparison.OrdinalIgnoreCase);
        AssertBlocked(unknownDestinationPlanet.Commands, "strategicMap.planet.viewDetail", StrategicMapCommandBlockReason.Unknown);
        var presence = mapSystem.FleetPresence.Single(x => x.OrbitalGroupId == group.Id);
        Assert.Equal(group.Id, presence.OrbitalGroupId);
        Assert.Equal(origin.Id, presence.PlanetId);
        Assert.Equal(SpaceAssetType.CargoCraft, presence.AssetType);
        Assert.Equal(OrbitalGroupStatus.Reserved, presence.Status);
        Assert.Null(presence.SensorProfile);
        var scoutPresence = mapSystem.FleetPresence.Single(x => x.OrbitalGroupId == scoutGroup.Id);
        Assert.Equal(SensorProfileClass.Orbital, scoutPresence.SensorProfile?.SensorClass);
        Assert.Equal(SensorProfileSourceKind.OrbitalGroup, scoutPresence.SensorProfile?.SourceKind);
        Assert.Contains(mapSystem.DetectionCoverage, x => x.SourceKind == DetectionCoverageSourceKind.OrbitalGroup && x.SourceId == scoutGroup.Id);
        var overlay = Assert.Single(mapSystem.TransferOverlays);
        Assert.Equal(transfer.Id, overlay.TransferId);
        Assert.Equal(2, overlay.AbstractDistanceUnits);
        Assert.Equal(destination.Id, overlay.DestinationPlanetId);
        Assert.Equal(OrbitalTransferStatus.Planned, overlay.Status);
        Assert.NotNull(overlay.InterceptionReadiness);
        Assert.Equal(InterceptionOpportunityStatus.ObservedOwnTransfer, overlay.InterceptionReadiness.OpportunityStatus);
        Assert.Equal([InterceptionOpportunityBlockReason.SelfObservedTransfer], overlay.InterceptionReadiness.BlockReasons);
        var originPlanet = mapSystem.Planets.Single(x => x.PlanetId == origin.Id);
        AssertAvailable(originPlanet.Commands, "fleet.travel.estimate");
        AssertAvailable(originPlanet.Commands, "fleet.transfer.create");
        Assert.Contains("existing fleet command path", originPlanet.Commands.Single(x => x.ActionKey == "fleet.transfer.create").Note, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(remoteMapSystem.FleetPresence);
        var remotePlanet = Assert.Single(remoteMapSystem.Planets);
        Assert.Equal(MapVisibilityLevel.Owned, remotePlanet.VisibilityLevel);
        Assert.Equal(ExplorationActionBlockReason.AlreadyOwned, remotePlanet.ExplorationPreview.BlockReason);
        AssertAvailable(remotePlanet.Commands, "fleet.travel.estimate");
        AssertAvailable(remotePlanet.Commands, "fleet.transfer.create");
    }

    [Fact]
    public async Task GetAsyncIncludesKnowledgeDerivedSystem()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Explored", 4, 5, 6);
        var planet = new Planet(Guid.NewGuid(), system.Id, "Surveyed", 1, PlanetType.Ice, 80);
        var hiddenPlanet = new Planet(Guid.NewGuid(), system.Id, "Hidden", 2, PlanetType.Barren, 60);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().AddRange(planet, hiddenPlanet);
        dbContext.ExplorationKnowledge.Add(ExplorationKnowledge.Create(civilizationId, system.Id, planet.Id, ExplorationKnowledgeSource.MissionCompletion, Guid.NewGuid(), DateTime.UtcNow));
        await dbContext.SaveChangesAsync();

        var mapSystem = Assert.Single((await CreateService(dbContext).GetAsync(new GetStrategicMapRequest(civilizationId))).Systems);

        Assert.Equal(system.Id, mapSystem.SystemId);
        Assert.Equal(MapVisibilityReason.ExploredSystem, mapSystem.VisibilityReason);
        AssertAvailable(mapSystem.Commands, "strategicMap.system.view");
        AssertBlocked(mapSystem.Commands, "exploration.preview", StrategicMapCommandBlockReason.ExplorationPreviewUnavailable);
        AssertBlocked(mapSystem.Commands, "exploration.mission.create", StrategicMapCommandBlockReason.ExplorationPreviewUnavailable);
        Assert.Equal(MapVisibilityReason.ExploredPlanet, mapSystem.Planets.Single(x => x.PlanetId == planet.Id).VisibilityReason);
        var hidden = mapSystem.Planets.Single(x => x.PlanetId == hiddenPlanet.Id);
        Assert.Equal(MapVisibilityLevel.Unknown, hidden.VisibilityLevel);
        Assert.Null(hidden.PlanetName);
        Assert.Null(hidden.PlanetType);
        Assert.Null(hidden.Size);
        Assert.Null(hidden.OrbitalSlot);
        Assert.Empty(hidden.DetectionCoverage);
    }

    [Fact]
    public async Task GetAsyncDoesNotMutatePersistedState()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var system = CreateSystem("Stable", 7, 8, 9);
        var planet = new Planet(Guid.NewGuid(), system.Id, "Quiet", 1, PlanetType.Barren, 70);
        var alliance = Alliance.Create("Void Council", "VC", AllianceStatus.Active, new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc));
        var partnerAlliance = Alliance.Create("Star Accord", "STAR", AllianceStatus.Active, new DateTime(2026, 6, 1, 12, 1, 0, DateTimeKind.Utc));
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().Add(planet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(planet.Id, civilizationId));
        dbContext.Alliances.AddRange(alliance, partnerAlliance);
        dbContext.AllianceMemberships.Add(AllianceMembership.Create(
            alliance.Id,
            civilizationId,
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Member,
            new DateTime(2026, 6, 1, 12, 5, 0, DateTimeKind.Utc)));
        dbContext.AlliancePacts.Add(AlliancePact.Create(
            alliance.Id,
            partnerAlliance.Id,
            AlliancePactType.TradeIntent,
            AlliancePactStatus.Archived,
            new DateTime(2026, 6, 1, 12, 6, 0, DateTimeKind.Utc)));
        await dbContext.SaveChangesAsync();
        var ownershipCount = await dbContext.Set<PlanetOwnership>().CountAsync();
        var knowledgeCount = await dbContext.ExplorationKnowledge.CountAsync();
        var allianceCount = await dbContext.Alliances.CountAsync();
        var membershipCount = await dbContext.AllianceMemberships.CountAsync();
        var pactCount = await dbContext.AlliancePacts.CountAsync();

        _ = await CreateService(dbContext).GetAsync(new GetStrategicMapRequest(civilizationId));

        Assert.Equal(0, dbContext.ChangeTracker.Entries().Count(x => x.State != EntityState.Unchanged));
        Assert.Equal(ownershipCount, await dbContext.Set<PlanetOwnership>().CountAsync());
        Assert.Equal(knowledgeCount, await dbContext.ExplorationKnowledge.CountAsync());
        Assert.Equal(allianceCount, await dbContext.Alliances.CountAsync());
        Assert.Equal(membershipCount, await dbContext.AllianceMemberships.CountAsync());
        Assert.Equal(pactCount, await dbContext.AlliancePacts.CountAsync());
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
