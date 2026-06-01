using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class AllianceReadinessSmokeTests
{
    [Fact]
    public async Task AllianceReadinessSurfacesRemainCoherentConservativeAndReadOnly()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 6, 1, 19, 0, 0, DateTimeKind.Utc);
        var player = PlayerProfile.Create(Guid.NewGuid().ToString(), "Alliance Smoke");
        var civilization = Civilization.Create(player.Id, "Alliance Readiness", CivilizationArchetype.Exploratory);
        var alliedCivilizationId = Guid.NewGuid();
        var contactedCivilizationId = Guid.NewGuid();
        var homeSystem = CreateSystem("Anchor", 1, 2, 3);
        var alliedSystem = CreateSystem("Cloister", 4, 5, 6);
        var homePlanet = new Planet(Guid.NewGuid(), homeSystem.Id, "Bastion", 1, PlanetType.Terran, 100, PlanetColonizationStatus.Colonized);
        var alliedPlanet = new Planet(Guid.NewGuid(), alliedSystem.Id, "Veil", 1, PlanetType.Desert, 80);
        var stockpile = PlanetResourceStockpile.Create(homePlanet.Id);
        stockpile.Increase(ResourceType.Credits, 75);
        stockpile.Increase(ResourceType.Gas, 30);
        var alliance = Alliance.Create("Void Council", "VC", AllianceStatus.Active, nowUtc);

        dbContext.PlayerProfiles.Add(player);
        dbContext.Civilizations.Add(civilization);
        dbContext.Alliances.Add(alliance);
        dbContext.AllianceMemberships.AddRange(
            AllianceMembership.Create(alliance.Id, civilization.Id, AllianceMembershipStatus.Active, AllianceMembershipRole.Leader, nowUtc.AddMinutes(5)),
            AllianceMembership.Create(alliance.Id, alliedCivilizationId, AllianceMembershipStatus.Active, AllianceMembershipRole.Member, nowUtc.AddMinutes(6)));
        dbContext.DiplomaticContacts.Add(DiplomaticContact.Create(
            civilization.Id,
            contactedCivilizationId,
            DiplomaticContactStatus.Friendly,
            nowUtc.AddMinutes(10),
            "manual-dev"));
        dbContext.Set<SolarSystem>().AddRange(homeSystem, alliedSystem);
        dbContext.Set<Planet>().AddRange(homePlanet, alliedPlanet);
        dbContext.Set<PlanetOwnership>().AddRange(
            PlanetOwnership.Create(homePlanet.Id, civilization.Id),
            PlanetOwnership.Create(alliedPlanet.Id, alliedCivilizationId));
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();

        var expectedCounts = (
            await dbContext.Alliances.CountAsync(),
            await dbContext.AllianceMemberships.CountAsync(),
            await dbContext.DiplomaticContacts.CountAsync(),
            await dbContext.Set<PlanetOwnership>().CountAsync(),
            await dbContext.PlanetResourceStockpiles.CountAsync(),
            await dbContext.ExplorationKnowledge.CountAsync(),
            await dbContext.ExplorationMissions.CountAsync());

        var allianceReadinessService = new AllianceReadinessQueryService(dbContext);
        var diplomacyService = new DiplomaticContactQueryService(dbContext);
        var visibilityService = new MapVisibilityService(dbContext);
        var strategicMapService = new StrategicMapService(
            dbContext,
            new SystemVisualStateService(dbContext, new PlanetVisualStateService(dbContext)),
            visibilityService,
            allianceReadinessQueryService: allianceReadinessService);

        var allianceReadiness = await allianceReadinessService.GetAsync(new GetAllianceReadinessRequest(civilization.Id));
        Assert.True(allianceReadiness.Succeeded);
        var allianceRow = Assert.Single(allianceReadiness.Alliances);
        Assert.Equal(alliance.Id, allianceRow.AllianceId);
        Assert.Equal(civilization.Id, allianceRow.Membership.CivilizationId);
        Assert.Equal(AllianceMembershipRole.Leader, allianceRow.Membership.Role);

        var diplomaticContacts = await diplomacyService.GetAsync(new GetDiplomaticContactsRequest(civilization.Id));
        Assert.True(diplomaticContacts.Succeeded);
        var contact = Assert.Single(diplomaticContacts.Contacts);
        Assert.Equal(contactedCivilizationId, contact.ContactedCivilizationId);
        Assert.DoesNotContain(allianceReadiness.Alliances, x => x.Membership.CivilizationId == contact.ContactedCivilizationId);

        var visibility = await visibilityService.GetAsync(new GetMapVisibilityRequest(civilization.Id));
        var visibleHomeSystem = visibility.Systems.Single(x => x.SystemId == homeSystem.Id);
        Assert.Equal(MapVisibilityReason.SystemContainsOwnedPlanet, visibleHomeSystem.VisibilityReason);
        var hiddenAlliedSystem = visibility.Systems.Single(x => x.SystemId == alliedSystem.Id);
        Assert.Equal(MapVisibilityLevel.Unknown, hiddenAlliedSystem.VisibilityLevel);
        Assert.False(hiddenAlliedSystem.IsVisible);

        var strategicMap = await strategicMapService.GetAsync(new GetStrategicMapRequest(civilization.Id));
        Assert.Contains(strategicMap.AllianceNotes, x => x.Note.Contains("shared visibility", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(strategicMap.AllianceNotes, x => x.Note.Contains("requesting civilization", StringComparison.OrdinalIgnoreCase));
        var strategicAlliance = Assert.Single(strategicMap.AllianceReadiness);
        Assert.Equal(alliance.Id, strategicAlliance.AllianceId);
        Assert.Equal(civilization.Id, strategicAlliance.Membership.CivilizationId);
        Assert.Single(strategicMap.DiplomaticContacts);
        Assert.DoesNotContain(strategicMap.Systems, x => x.SystemId == alliedSystem.Id);
        var mapHomeSystem = Assert.Single(strategicMap.Systems);
        Assert.Equal(homeSystem.Id, mapHomeSystem.SystemId);
        Assert.DoesNotContain(mapHomeSystem.Planets, x => x.PlanetId == alliedPlanet.Id);

        var manifestActions = new DevStrategicMapActionManifestService().Get().Actions;
        Assert.Contains(manifestActions, x => x.ActionKey == "alliance.readiness.read" && x.Route == "/api/dev/strategic-map/alliances/readiness" && x.IsReadOnly);
        Assert.Contains(manifestActions, x => x.ActionKey == "diplomacy.contact.read" && x.IsReadOnly);

        Assert.Equal(75, dbContext.PlanetResourceStockpiles.AsNoTracking().Single().Credits);
        Assert.Equal(30, dbContext.PlanetResourceStockpiles.AsNoTracking().Single().Gas);
        Assert.Equal(0, dbContext.ChangeTracker.Entries().Count(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted));
        Assert.Equal(expectedCounts, (
            await dbContext.Alliances.CountAsync(),
            await dbContext.AllianceMemberships.CountAsync(),
            await dbContext.DiplomaticContacts.CountAsync(),
            await dbContext.Set<PlanetOwnership>().CountAsync(),
            await dbContext.PlanetResourceStockpiles.CountAsync(),
            await dbContext.ExplorationKnowledge.CountAsync(),
            await dbContext.ExplorationMissions.CountAsync()));
        AssertNoFutureAllianceSystems(dbContext);
    }

    private static void AssertNoFutureAllianceSystems(VoidEmpiresDbContext dbContext)
    {
        var entityNames = dbContext.Model.GetEntityTypes().Select(x => x.ClrType.Name).ToArray();
        var blockedTerms = new[]
        {
            "AlliancePermission", "AllianceInvitation", "Treaty", "Trade", "War",
            "Espionage", "Combat", "SharedVisibility", "FinalUi"
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
