namespace VoidEmpires.Domain.Galaxy;

public sealed class Planet
{
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

    public Guid Id { get; }

    public Guid SolarSystemId { get; }

    public string Name { get; }

    public int OrbitalSlot { get; }

    public PlanetType PlanetType { get; }

    public int Size { get; }

    public PlanetColonizationStatus ColonizationStatus { get; }

    public static Planet Create(
        Guid solarSystemId,
        string name,
        int orbitalSlot,
        PlanetType planetType,
        int size,
        PlanetColonizationStatus colonizationStatus = PlanetColonizationStatus.Uncolonized) =>
        new(Guid.NewGuid(), solarSystemId, name, orbitalSlot, planetType, size, colonizationStatus);
}
