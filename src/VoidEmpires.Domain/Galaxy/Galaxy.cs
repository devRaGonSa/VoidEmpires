namespace VoidEmpires.Domain.Galaxy;

public sealed class Galaxy
{
    private readonly List<SolarSystem> _solarSystems = [];

    public Galaxy(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Galaxy id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Galaxy name must not be empty.", nameof(name));
        }

        Id = id;
        Name = name.Trim();
    }

    public Guid Id { get; }

    public string Name { get; }

    public IReadOnlyCollection<SolarSystem> SolarSystems => _solarSystems;

    public void AddSolarSystem(SolarSystem solarSystem)
    {
        ArgumentNullException.ThrowIfNull(solarSystem);

        if (_solarSystems.Any(existing => existing.Coordinates == solarSystem.Coordinates))
        {
            throw new InvalidOperationException("A solar system with the same coordinates already exists.");
        }

        _solarSystems.Add(solarSystem);
    }

    public static Galaxy Create(string name) => new(Guid.NewGuid(), name);
}
