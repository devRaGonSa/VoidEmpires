namespace VoidEmpires.Domain.Galaxy;

public sealed class Star
{
    public Star(Guid id, Guid solarSystemId, string name, StarType starType)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Star id must not be empty.", nameof(id));
        }

        if (solarSystemId == Guid.Empty)
        {
            throw new ArgumentException("Solar system id must not be empty.", nameof(solarSystemId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Star name must not be empty.", nameof(name));
        }

        Id = id;
        SolarSystemId = solarSystemId;
        Name = name.Trim();
        StarType = starType;
    }

    public Guid Id { get; }

    public Guid SolarSystemId { get; }

    public string Name { get; }

    public StarType StarType { get; }

    public static Star Create(Guid solarSystemId, string name, StarType starType) =>
        new(Guid.NewGuid(), solarSystemId, name, starType);
}
