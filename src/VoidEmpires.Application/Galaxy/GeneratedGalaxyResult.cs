using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Application.Galaxy;

public sealed record GeneratedGalaxyResult(Domain.Galaxy.Galaxy Galaxy)
{
    public IReadOnlyCollection<SolarSystem> SolarSystems => Galaxy.SolarSystems;

    public int SolarSystemCount => Galaxy.SolarSystems.Count;

    public int PlanetCount => Galaxy.SolarSystems.Sum(system => system.Planets.Count);
}
