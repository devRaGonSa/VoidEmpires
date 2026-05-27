namespace VoidEmpires.Domain.Galaxy;

public sealed class SolarSystem
{
    private readonly List<Planet> _planets = [];

    private SolarSystem()
    {
        Name = string.Empty;
        Star = null!;
    }

    public SolarSystem(Guid id, Guid galaxyId, string name, GalaxyCoordinates coordinates, Star star)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Solar system id must not be empty.", nameof(id));
        }

        if (galaxyId == Guid.Empty)
        {
            throw new ArgumentException("Galaxy id must not be empty.", nameof(galaxyId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Solar system name must not be empty.", nameof(name));
        }

        Id = id;
        GalaxyId = galaxyId;
        Name = name.Trim();
        Coordinates = coordinates;
        Star = star ?? throw new ArgumentNullException(nameof(star));
    }

    public Guid Id { get; private set; }

    public Guid GalaxyId { get; private set; }

    public string Name { get; private set; }

    public GalaxyCoordinates Coordinates { get; private set; }

    public Star Star { get; private set; }

    public IReadOnlyCollection<Planet> Planets => _planets;

    public void AddPlanet(Planet planet)
    {
        ArgumentNullException.ThrowIfNull(planet);

        if (_planets.Any(existing => existing.OrbitalSlot == planet.OrbitalSlot))
        {
            throw new InvalidOperationException("A planet with the same orbital slot already exists.");
        }

        _planets.Add(planet);
    }
}
