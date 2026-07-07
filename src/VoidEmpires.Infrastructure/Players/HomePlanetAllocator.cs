using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Players;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Players;

public sealed class HomePlanetAllocator(VoidEmpiresDbContext dbContext) : IHomePlanetAllocator
{
    private const string BootstrapGalaxyName = "Account Bootstrap Galaxy";

    public async Task<Planet> AllocateAsync(
        string homePlanetName,
        CancellationToken cancellationToken = default)
    {
        var candidate = await dbContext.Planets
            .Where(planet =>
                planet.PlanetType == PlanetType.Terran &&
                !dbContext.PlanetOwnerships.Any(ownership => ownership.PlanetId == planet.Id))
            .OrderBy(planet => planet.SolarSystemId)
            .ThenBy(planet => planet.OrbitalSlot)
            .FirstOrDefaultAsync(cancellationToken);

        if (candidate is not null)
        {
            return candidate;
        }

        var galaxy = await dbContext.Galaxies
            .FirstOrDefaultAsync(x => x.Name == BootstrapGalaxyName, cancellationToken);
        if (galaxy is null)
        {
            galaxy = Galaxy.Create(BootstrapGalaxyName);
            dbContext.Galaxies.Add(galaxy);
        }

        var usedXCoordinates = await dbContext.SolarSystems
            .Where(system => system.GalaxyId == galaxy.Id && system.CoordinateY == 0 && system.CoordinateZ == 0)
            .Select(system => system.CoordinateX)
            .ToListAsync(cancellationToken);
        var coordinateX = 0;
        while (usedXCoordinates.Contains(coordinateX))
        {
            coordinateX++;
        }

        var solarSystemId = Guid.NewGuid();
        var solarSystem = new SolarSystem(
            solarSystemId,
            galaxy.Id,
            $"{homePlanetName} System",
            new GalaxyCoordinates(coordinateX, 0, 0),
            Star.Create(solarSystemId, $"{homePlanetName} Star", StarType.YellowDwarf));
        var homePlanet = Planet.Create(solarSystemId, homePlanetName, 1, PlanetType.Terran, 118, PlanetColonizationStatus.Colonized);

        dbContext.SolarSystems.Add(solarSystem);
        dbContext.Planets.Add(homePlanet);
        return homePlanet;
    }
}
