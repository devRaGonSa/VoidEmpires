namespace VoidEmpires.Domain.Buildings;

public sealed class PlanetBuildingCapacity
{
    private PlanetBuildingCapacity() { }

    private PlanetBuildingCapacity(Guid planetId, int baseCapacity, int bonusCapacity = 0)
    {
        if (planetId == Guid.Empty)
        {
            throw new ArgumentException("Planet id is required.");
        }

        if (baseCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(baseCapacity), "Base capacity must be positive.");
        }

        if (bonusCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bonusCapacity), "Bonus capacity cannot be negative.");
        }

        Id = Guid.NewGuid();
        PlanetId = planetId;
        BaseCapacity = baseCapacity;
        BonusCapacity = bonusCapacity;
    }

    public Guid Id { get; private set; }
    public Guid PlanetId { get; private set; }
    public int BaseCapacity { get; private set; }
    public int BonusCapacity { get; private set; }
    public int TotalCapacity => BaseCapacity + BonusCapacity;

    public static PlanetBuildingCapacity Create(Guid planetId, int baseCapacity, int bonusCapacity = 0)
        => new(planetId, baseCapacity, bonusCapacity);

    public bool CanFit(int usedCapacity, int requestedFootprint)
    {
        if (usedCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(usedCapacity), "Used capacity cannot be negative.");
        }

        if (requestedFootprint <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedFootprint), "Requested footprint must be positive.");
        }

        return usedCapacity + requestedFootprint <= TotalCapacity;
    }
}
