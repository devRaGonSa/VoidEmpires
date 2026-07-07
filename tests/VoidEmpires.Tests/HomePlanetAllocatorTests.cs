using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Players;

namespace VoidEmpires.Tests;

public class HomePlanetAllocatorTests
{
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task AllocateAsyncCreatesDefaultHomeWorldForEmptyUniverse()
    {
        await using var dbContext = CreateDbContext();
        var allocator = new HomePlanetAllocator(dbContext);

        var planet = await allocator.AllocateAsync("Nova Prime");
        await dbContext.SaveChangesAsync();

        Assert.Equal("Nova Prime", planet.Name);
        Assert.True(await dbContext.Galaxies.AnyAsync(x => x.Name == "Account Bootstrap Galaxy"));
        Assert.True(await dbContext.SolarSystems.AnyAsync(x => x.Id == planet.SolarSystemId && x.CoordinateX == 0));
    }

    [Fact]
    public async Task AllocateAsyncReturnsExistingUnownedTerranPlanet()
    {
        await using var dbContext = CreateDbContext();
        var galaxy = Galaxy.Create("Shared Galaxy");
        var systemId = Guid.NewGuid();
        var system = new SolarSystem(systemId, galaxy.Id, "Helios Gate", new GalaxyCoordinates(0, 0, 0), Star.Create(systemId, "Helios Star", StarType.YellowDwarf));
        var planet = Planet.Create(systemId, "Open World", 2, PlanetType.Terran, 90);
        dbContext.Galaxies.Add(galaxy);
        dbContext.SolarSystems.Add(system);
        dbContext.Planets.Add(planet);
        await dbContext.SaveChangesAsync();

        var allocated = await new HomePlanetAllocator(dbContext).AllocateAsync("Ignored Name");

        Assert.Equal(planet.Id, allocated.Id);
    }

    [Fact]
    public async Task AllocateAsyncSkipsOwnedPlanets()
    {
        await using var dbContext = CreateDbContext();
        await new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));
        var allocator = new HomePlanetAllocator(dbContext);

        var planet = await allocator.AllocateAsync("New Home");

        Assert.NotEqual(SeedOwnedPlanetId, planet.Id);
        Assert.False(await dbContext.PlanetOwnerships.AnyAsync(x => x.PlanetId == planet.Id));
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
