using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Application.Players;

public static class StartingHomeWorldBaseline
{
    public const decimal StartingCredits = 220m;
    public const decimal StartingMetal = 320m;
    public const decimal StartingCrystal = 220m;
    public const decimal StartingGas = 120m;

    public const decimal BaseCreditsPerHour = 18m;
    public const decimal BaseMetalPerHour = 14m;
    public const decimal BaseCrystalPerHour = 6m;
    public const decimal BaseGasPerHour = 3m;

    public static PlanetResourceStockpile CreateResourceStockpile(Guid planetId)
    {
        var stockpile = PlanetResourceStockpile.Create(planetId);
        stockpile.Increase(ResourceType.Credits, StartingCredits);
        stockpile.Increase(ResourceType.Metal, StartingMetal);
        stockpile.Increase(ResourceType.Crystal, StartingCrystal);
        stockpile.Increase(ResourceType.Gas, StartingGas);
        return stockpile;
    }

    public static PlanetProductionProfile CreateProductionProfile(Guid planetId) =>
        PlanetProductionProfile.Create(
            planetId,
            BaseCreditsPerHour,
            BaseMetalPerHour,
            BaseCrystalPerHour,
            BaseGasPerHour);

    public static CreateStartingCivilizationResourceSnapshot CreateResourceSnapshot() =>
        new(StartingCredits, StartingMetal, StartingCrystal, StartingGas);
}
