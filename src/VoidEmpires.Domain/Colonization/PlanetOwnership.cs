namespace VoidEmpires.Domain.Colonization;

public sealed class PlanetOwnership
{
    private PlanetOwnership() { }

    private PlanetOwnership(Guid planetId, Guid civilizationId)
    {
        if (planetId == Guid.Empty) throw new ArgumentException("Planet id is required.");
        if (civilizationId == Guid.Empty) throw new ArgumentException("Civilization id is required.");

        Id = Guid.NewGuid();
        PlanetId = planetId;
        CivilizationId = civilizationId;
        Status = PlanetControlStatus.Active;
        ClaimedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid PlanetId { get; private set; }
    public Guid CivilizationId { get; private set; }
    public PlanetControlStatus Status { get; private set; }
    public DateTime ClaimedAtUtc { get; private set; }

    public static PlanetOwnership Create(Guid planetId, Guid civilizationId)
        => new(planetId, civilizationId);
}
