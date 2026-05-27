namespace VoidEmpires.Domain.Buildings;

public sealed class PlanetBuilding
{
    private PlanetBuilding() { }

    private PlanetBuilding(
        Guid planetId,
        BuildingType buildingType,
        int level,
        int footprint)
    {
        if (planetId == Guid.Empty)
        {
            throw new ArgumentException("Planet id is required.");
        }

        if (level <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(level));
        }

        if (footprint <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(footprint));
        }

        Id = Guid.NewGuid();
        PlanetId = planetId;
        BuildingType = buildingType;
        Level = level;
        Footprint = footprint;
    }

    public Guid Id { get; private set; }
    public Guid PlanetId { get; private set; }
    public BuildingType BuildingType { get; private set; }
    public int Level { get; private set; }
    public int Footprint { get; private set; }

    public static PlanetBuilding Create(
        Guid planetId,
        BuildingType buildingType,
        int level,
        int footprint)
        => new(planetId, buildingType, level, footprint);

    public void Upgrade()
    {
        Level++;
    }
}
