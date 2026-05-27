using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Colonization;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Colonization;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class PlanetColonizationServiceTests
{
    [Fact]
    public async Task ColonizeAsyncCreatesPlanetOwnership()
    {
        await using var dbContext = CreateDbContext();
        var (planetId, civilizationId) = await SeedPlanetAndCivilizationAsync(dbContext);
        var service = new PlanetColonizationService(dbContext);

        var result = await service.ColonizeAsync(new ColonizePlanetRequest(planetId, civilizationId));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.OwnershipId);
        Assert.Empty(result.Errors);
        Assert.Equal(1, await dbContext.PlanetOwnerships.CountAsync());
    }

    [Fact]
    public async Task ColonizeAsyncRejectsMissingPlanet()
    {
        await using var dbContext = CreateDbContext();
        var (_, civilizationId) = await SeedPlanetAndCivilizationAsync(dbContext);
        var service = new PlanetColonizationService(dbContext);

        var result = await service.ColonizeAsync(new ColonizePlanetRequest(Guid.NewGuid(), civilizationId));

        Assert.False(result.Succeeded);
        Assert.Equal(["Planet was not found."], result.Errors);
    }

    [Fact]
    public async Task ColonizeAsyncRejectsMissingCivilization()
    {
        await using var dbContext = CreateDbContext();
        var (planetId, _) = await SeedPlanetAndCivilizationAsync(dbContext);
        var service = new PlanetColonizationService(dbContext);

        var result = await service.ColonizeAsync(new ColonizePlanetRequest(planetId, Guid.NewGuid()));

        Assert.False(result.Succeeded);
        Assert.Equal(["Civilization was not found."], result.Errors);
    }

    [Fact]
    public async Task ColonizeAsyncRejectsAlreadyControlledPlanet()
    {
        await using var dbContext = CreateDbContext();
        var (planetId, civilizationId) = await SeedPlanetAndCivilizationAsync(dbContext);
        var service = new PlanetColonizationService(dbContext);

        var first = await service.ColonizeAsync(new ColonizePlanetRequest(planetId, civilizationId));
        var duplicate = await service.ColonizeAsync(new ColonizePlanetRequest(planetId, civilizationId));

        Assert.True(first.Succeeded);
        Assert.False(duplicate.Succeeded);
        Assert.Equal(["Planet is already controlled."], duplicate.Errors);
        Assert.Equal(1, await dbContext.PlanetOwnerships.CountAsync());
    }

    [Fact]
    public async Task ColonizeAsyncRejectsEmptyIds()
    {
        await using var dbContext = CreateDbContext();
        var service = new PlanetColonizationService(dbContext);

        var missingPlanet = await service.ColonizeAsync(new ColonizePlanetRequest(Guid.Empty, Guid.NewGuid()));
        var missingCivilization = await service.ColonizeAsync(new ColonizePlanetRequest(Guid.NewGuid(), Guid.Empty));

        Assert.False(missingPlanet.Succeeded);
        Assert.Equal(["Planet id is required."], missingPlanet.Errors);
        Assert.False(missingCivilization.Succeeded);
        Assert.Equal(["Civilization id is required."], missingCivilization.Errors);
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }

    private static async Task<(Guid PlanetId, Guid CivilizationId)> SeedPlanetAndCivilizationAsync(VoidEmpiresDbContext dbContext)
    {
        var galaxy = Domain.Galaxy.Galaxy.Create("Void Prime");
        var solarSystemId = Guid.NewGuid();
        var star = Star.Create(solarSystemId, "Void Prime Star", StarType.YellowDwarf);
        var solarSystem = new SolarSystem(
            solarSystemId,
            galaxy.Id,
            "Void Prime System 0001",
            new GalaxyCoordinates(1, 2, 3),
            star);
        var planet = Planet.Create(solarSystem.Id, "Void Prime I", 1, PlanetType.Terrestrial, 8000);
        solarSystem.AddPlanet(planet);
        galaxy.AddSolarSystem(solarSystem);

        var profile = PlayerProfile.Create("user-1", "Player One");
        var civilization = Civilization.Create(profile.Id, "Solar Dominion", CivilizationArchetype.Balanced, planet.Id);
        profile.AddCivilization(civilization);

        dbContext.Galaxies.Add(galaxy);
        dbContext.PlayerProfiles.Add(profile);
        await dbContext.SaveChangesAsync();

        return (planet.Id, civilization.Id);
    }
}
