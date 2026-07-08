using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Infrastructure.Buildings;

internal static class PlanetBuildingCapacityDefaults
{
    public const int StartingBaseCapacity = 120;

    public static PlanetBuildingCapacity CreateForPlanet(Planet planet) =>
        PlanetBuildingCapacity.Create(planet.Id, ResolveBaseCapacity(planet.Size));

    public static PlanetBuildingCapacity Create(Guid planetId, int planetSize) =>
        PlanetBuildingCapacity.Create(planetId, ResolveBaseCapacity(planetSize));

    private static int ResolveBaseCapacity(int planetSize)
    {
        if (planetSize >= StartingBaseCapacity && planetSize <= 240)
        {
            return planetSize;
        }

        return StartingBaseCapacity;
    }
}
