using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class ExplorationMissionCreateServiceTests
{
    [Fact]
    public async Task CreateAsyncCreatesMissionForUnknownSystem()
    {
        await using var dbContext = CreateDbContext();
        var civilization = AddCivilization(dbContext);
        var system = CreateSystem("Unknown", 1, 2, 3);
        dbContext.Set<SolarSystem>().Add(system);
        await dbContext.SaveChangesAsync();
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

        var result = await CreateService(dbContext).CreateAsync(new CreateExplorationMissionRequest(
            civilization.Id,
            system.Id,
            null,
            requestedAtUtc));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Mission);
        Assert.Equal(ExplorationMissionStatus.Planned, result.Mission.Status);
        Assert.Equal(requestedAtUtc.AddMinutes(30), result.Mission.DueAtUtc);
        var mission = await dbContext.ExplorationMissions.SingleAsync();
        Assert.Equal(system.Id, mission.TargetSystemId);
        Assert.Null(mission.TargetPlanetId);
    }

    [Fact]
    public async Task CreateAsyncCreatesMissionForUnknownPlanet()
    {
        await using var dbContext = CreateDbContext();
        var civilization = AddCivilization(dbContext);
        var system = CreateSystem("Unknown", 1, 2, 3);
        var planet = new Planet(Guid.NewGuid(), system.Id, "Hidden", 1, PlanetType.Barren, 60);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().Add(planet);
        await dbContext.SaveChangesAsync();
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

        var result = await CreateService(dbContext).CreateAsync(new CreateExplorationMissionRequest(
            civilization.Id,
            system.Id,
            planet.Id,
            requestedAtUtc));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Mission);
        Assert.Equal(planet.Id, result.Mission.TargetPlanetId);
        Assert.Equal(requestedAtUtc.AddMinutes(45), result.Mission.DueAtUtc);
    }

    [Fact]
    public async Task CreateAsyncRejectsVisibleOrOwnedTarget()
    {
        await using var dbContext = CreateDbContext();
        var civilization = AddCivilization(dbContext);
        var system = CreateSystem("Visible", 1, 2, 3);
        var ownedPlanet = new Planet(Guid.NewGuid(), system.Id, "Home", 1, PlanetType.Terran, 80);
        dbContext.Set<SolarSystem>().Add(system);
        dbContext.Set<Planet>().Add(ownedPlanet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(ownedPlanet.Id, civilization.Id));
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).CreateAsync(new CreateExplorationMissionRequest(
            civilization.Id,
            system.Id,
            null,
            new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc)));

        Assert.False(result.Succeeded);
        Assert.True(result.IsConflict);
        Assert.Contains("Target system is not eligible for exploration.", result.Errors);
        Assert.Empty(await dbContext.ExplorationMissions.ToListAsync());
    }

    [Fact]
    public async Task CreateAsyncRejectsPlanetOutsideTargetSystem()
    {
        await using var dbContext = CreateDbContext();
        var civilization = AddCivilization(dbContext);
        var targetSystem = CreateSystem("Target", 1, 2, 3);
        var otherSystem = CreateSystem("Other", 4, 5, 6);
        var otherPlanet = new Planet(Guid.NewGuid(), otherSystem.Id, "Other", 1, PlanetType.Ice, 60);
        dbContext.Set<SolarSystem>().AddRange(targetSystem, otherSystem);
        dbContext.Set<Planet>().Add(otherPlanet);
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).CreateAsync(new CreateExplorationMissionRequest(
            civilization.Id,
            targetSystem.Id,
            otherPlanet.Id,
            new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc)));

        Assert.False(result.Succeeded);
        Assert.False(result.IsConflict);
        Assert.Contains("Target planet was not found in the target system.", result.Errors);
    }

    [Fact]
    public async Task CreateAsyncDoesNotRevealVisibility()
    {
        await using var dbContext = CreateDbContext();
        var civilization = AddCivilization(dbContext);
        var system = CreateSystem("Unknown", 1, 2, 3);
        dbContext.Set<SolarSystem>().Add(system);
        await dbContext.SaveChangesAsync();

        _ = await CreateService(dbContext).CreateAsync(new CreateExplorationMissionRequest(
            civilization.Id,
            system.Id,
            null,
            new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc)));

        Assert.Equal(0, await dbContext.Set<PlanetOwnership>().CountAsync());
        var visibility = await new MapVisibilityService(dbContext).GetAsync(new GetMapVisibilityRequest(civilization.Id));
        Assert.Equal(MapVisibilityLevel.Unknown, Assert.Single(visibility.Systems).VisibilityLevel);
    }

    private static ExplorationMissionCreateService CreateService(VoidEmpiresDbContext dbContext) =>
        new(dbContext, new ExplorationActionPreviewService(new MapVisibilityService(dbContext)));

    private static Civilization AddCivilization(VoidEmpiresDbContext dbContext)
    {
        var profile = PlayerProfile.Create(Guid.NewGuid().ToString(), "Player");
        var civilization = Civilization.Create(profile.Id, $"Civ {Guid.NewGuid():N}", CivilizationArchetype.Exploratory);
        dbContext.PlayerProfiles.Add(profile);
        dbContext.Civilizations.Add(civilization);
        return civilization;
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
