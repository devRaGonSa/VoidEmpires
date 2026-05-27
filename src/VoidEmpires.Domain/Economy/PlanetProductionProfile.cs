namespace VoidEmpires.Domain.Economy;

public sealed class PlanetProductionProfile
{
    private PlanetProductionProfile() { }

    private PlanetProductionProfile(
        Guid planetId,
        decimal creditsPerHour,
        decimal metalPerHour,
        decimal crystalPerHour,
        decimal gasPerHour)
    {
        if (planetId == Guid.Empty)
        {
            throw new ArgumentException("Planet id is required.");
        }

        if (creditsPerHour < 0 || metalPerHour < 0 || crystalPerHour < 0 || gasPerHour < 0)
        {
            throw new ArgumentException("Production values cannot be negative.");
        }

        Id = Guid.NewGuid();
        PlanetId = planetId;
        CreditsPerHour = creditsPerHour;
        MetalPerHour = metalPerHour;
        CrystalPerHour = crystalPerHour;
        GasPerHour = gasPerHour;
    }

    public Guid Id { get; private set; }
    public Guid PlanetId { get; private set; }
    public decimal CreditsPerHour { get; private set; }
    public decimal MetalPerHour { get; private set; }
    public decimal CrystalPerHour { get; private set; }
    public decimal GasPerHour { get; private set; }

    public static PlanetProductionProfile Create(
        Guid planetId,
        decimal creditsPerHour,
        decimal metalPerHour,
        decimal crystalPerHour,
        decimal gasPerHour) => new(planetId, creditsPerHour, metalPerHour, crystalPerHour, gasPerHour);
}
