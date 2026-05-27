namespace VoidEmpires.Domain.Galaxy;

public sealed class Planet
{
    private Planet()
    {
        Name = string.Empty;
    }

    public Planet(
        Guid id,
        Guid solarSystemId,
        string name,
        int orbitalSlot,
        PlanetType planetType,
        int size,
        PlanetColonizationStatus colonizationStatus = PlanetColonizationStatus.Uncolonized)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Planet id must not be empty.", nameof(id));
        }

        if (solarSystemId == Guid.Empty)
        {
            throw new ArgumentException("Solar system id must not be empty.", nameof(solarSystemId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Planet name must not be empty.", nameof(name));
        }

        if (orbitalSlot <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(orbitalSlot), "Planet orbital slot must be positive.");
        }

        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Planet size must be positive.");
        }

        Id = id;
        SolarSystemId = solarSystemId;
        Name = name.Trim();
        OrbitalSlot = orbitalSlot;
        PlanetType = planetType;
        Size = size;
        ColonizationStatus = colonizationStatus;
    }

    public Guid Id { get; private set; }

    public Guid SolarSystemId { get; private set; }

    public string Name { get; private set; }

    public int OrbitalSlot { get; private set; }

    public PlanetType PlanetType { get; private set; }

    public int Size { get; private set; }

    public PlanetColonizationStatus ColonizationStatus { get; private set; }

    public static Planet Create(
        Guid solarSystemId,
        string name,
        int orbitalSlot,
        PlanetType planetType,
        int size,
        PlanetColonizationStatus colonizationStatus = PlanetColonizationStatus.Uncolonized) =>
        new(Guid.NewGuid(), solarSystemId, name, orbitalSlot, planetType, size, colonizationStatus);
}
