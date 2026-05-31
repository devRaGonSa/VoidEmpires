using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Domain.Fleets;

public static class OrbitalTravelEstimator
{
    public const int AbstractDistanceUnitsPerPlanetTransfer = 1;
    public const int BaseGasCostPerDistanceUnit = 5;
    public static readonly TimeSpan DurationPerAbstractDistanceUnit = TimeSpan.FromHours(1);

    public static int EstimateAbstractDistanceUnits(Guid currentPlanetId, Guid destinationPlanetId)
    {
        if (currentPlanetId == Guid.Empty)
        {
            throw new ArgumentException("Current planet id is required.", nameof(currentPlanetId));
        }

        if (destinationPlanetId == Guid.Empty)
        {
            throw new ArgumentException("Destination planet id is required.", nameof(destinationPlanetId));
        }

        if (currentPlanetId == destinationPlanetId)
        {
            throw new ArgumentException("Destination planet must be different from the current planet.", nameof(destinationPlanetId));
        }

        return AbstractDistanceUnitsPerPlanetTransfer;
    }

    public static TimeSpan EstimateTravelDuration(int abstractDistanceUnits)
    {
        if (abstractDistanceUnits <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(abstractDistanceUnits));
        }

        return TimeSpan.FromTicks(DurationPerAbstractDistanceUnit.Ticks * abstractDistanceUnits);
    }

    public static OrbitalTravelCostEstimate EstimateTravelCost(
        SpaceAssetType assetType,
        int quantity,
        int abstractDistanceUnits)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (abstractDistanceUnits <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(abstractDistanceUnits));
        }

        var multiplier = GetAssetFuelMultiplier(assetType);
        var gas = BaseGasCostPerDistanceUnit * multiplier * quantity * abstractDistanceUnits;

        return new OrbitalTravelCostEstimate([
            new OrbitalTravelCostComponent(ResourceType.Gas, gas)
        ]);
    }

    private static int GetAssetFuelMultiplier(SpaceAssetType assetType) => assetType switch
    {
        SpaceAssetType.ScoutCraft => 1,
        SpaceAssetType.CargoCraft => 2,
        SpaceAssetType.EscortCraft => 3,
        SpaceAssetType.ColonyCraft => 4,
        _ => 1
    };
}

public sealed record OrbitalTravelCostEstimate(IReadOnlyList<OrbitalTravelCostComponent> Components);

public sealed record OrbitalTravelCostComponent(ResourceType ResourceType, int Amount);
