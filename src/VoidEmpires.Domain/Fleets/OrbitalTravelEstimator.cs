using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Domain.Fleets;

public static class OrbitalTravelEstimator
{
    public const int AbstractDistanceUnitsPerPlanetTransfer = 1;
    public const decimal CreditsPerAbstractDistanceUnit = 5;
    public const decimal GasPerAbstractDistanceUnit = 2;

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

    public static OrbitalTravelEstimate Estimate(
        Guid currentPlanetId,
        Guid destinationPlanetId,
        SpaceAssetType assetType)
    {
        var distance = EstimateAbstractDistanceUnits(currentPlanetId, destinationPlanetId);

        return Estimate(distance, assetType);
    }

    public static OrbitalTravelEstimate Estimate(int abstractDistanceUnits, SpaceAssetType assetType)
    {
        var duration = EstimateTravelDuration(abstractDistanceUnits);
        var costMultiplier = GetCostMultiplier(assetType);

        return new OrbitalTravelEstimate(
            abstractDistanceUnits,
            duration,
            [
                new OrbitalTravelResourceCost(
                    ResourceType.Credits,
                    CreditsPerAbstractDistanceUnit * abstractDistanceUnits * costMultiplier),
                new OrbitalTravelResourceCost(
                    ResourceType.Gas,
                    GasPerAbstractDistanceUnit * abstractDistanceUnits * costMultiplier)
            ]);
    }

    public static decimal GetCostMultiplier(SpaceAssetType assetType)
    {
        return assetType switch
        {
            SpaceAssetType.ScoutCraft => 1m,
            SpaceAssetType.CargoCraft => 1.5m,
            SpaceAssetType.EscortCraft => 2m,
            SpaceAssetType.ColonyCraft => 3m,
            _ => throw new ArgumentOutOfRangeException(nameof(assetType))
        };
    }
}
